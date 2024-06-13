using Microsoft.AspNetCore.Mvc;
using BlockchainDemo.Models;
using BlockchainDemo.Services;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System;


namespace BlockchainDemo.Controllers {

    public class P2PMethors {

        public static List<string> node = new List<string>();
        public string filePath = "IPS.txt";

        /*
        * 
        * Esta função Atualiza o Arquivo IPS.txt com os dados da Variável node.
        *
        * @returns {void}
        */
        public void UpdateIPS() {

            if (!File.Exists(filePath)) {
                File.Create(filePath).Close();
            }

            node.AddRange(File.ReadAllLines(filePath));

        }

        /*
        * 
        * Esta função Envia via TCP/IP a Blockchain Como um Hexadecimal para todos os IPs da Variável node.
        * 
        * @returns {void}
        */
        public void SendBlockchain() {

            UpdateIPS();

            var BlockServices = new BlockServices();
            var MainServices = new MainServices();

            for (int i = 0; i < node.Count; i++) {

                int serverPort = 7001;
                // Convertendo lista para uma representação Hexadecimal
                byte[] hex = MainServices.ConvertListToHexadecimal(BlockServices.chain);

                try {
                    // Criar uma instância TcpClient e se conecta ao servidor
                    using (TcpClient client = new TcpClient(node[i], serverPort)) {
                        // Obtém o stream de rede associado ao TcpClient
                        NetworkStream stream = client.GetStream();
                        // Enviar o Hexadecimal
                        string hexString = BitConverter.ToString(hex).Replace("-", "");
                        byte[] data = Encoding.ASCII.GetBytes(hexString);
                        stream.Write(data, 0, data.Length);
                    }
                } catch {
                    // Excluindo o Nó caso ele não exista
                    node.RemoveAt(i);
                    File.WriteAllLines(filePath, node.ToArray());
                }
            }
        }

        /*
        * 
        * Esta função Envia via TCP/IP a Mempool Como um Hexadecimal para todos os IPs da Variável node.
        * 
        * @returns {void}
        */
        public void SendMempool() {

            UpdateIPS();

            var MempoolServices = new MempoolServices();
            var MainServices = new MainServices();

            for (int i = 0; i < node.Count; i++) {

                int serverPort = 7001;
                // Convertendo lista para uma representação Hexadecimal
                byte[] hex = MainServices.ConvertListToHexadecimal(MempoolServices.mempool);

                try {
                    // Criar uma instância TcpClient e se conecta ao servidor
                    using (TcpClient client = new TcpClient(node[i], serverPort)) {
                        // Obtém o stream de rede associado ao TcpClient
                        NetworkStream stream = client.GetStream();
                        // Enviar o Hexadecimal
                        string hexString = BitConverter.ToString(hex).Replace("-", "");
                        byte[] data = Encoding.ASCII.GetBytes(hexString);
                        stream.Write(data, 0, data.Length);
                    }
                } catch {
                    // Excluindo o Nó caso ele não exista
                    node.RemoveAt(i);
                    File.WriteAllLines(filePath, node.ToArray());
                }
            }
        }
    }


    [ApiController]
    [Route("[controller]")]
    public class P2PController : ControllerBase {

        [HttpPost]
        public IActionResult ConnectionNode([FromBody] dynamic dadosObtidos) {
            var BlockServices = new BlockServices();
            var MempoolServices = new MempoolServices();
            var P2PMethors = new P2PMethors();
            bool is_blockchain = false;

            // Convertendo os Dados Obtidos para JSON
            string jsonString = System.Text.Json.JsonSerializer.Serialize(dadosObtidos);
            dynamic? dados = JsonConvert.DeserializeObject<dynamic>(jsonString);

            // Obtendo a Lista de Transações ou a Lista de Blocos
            List<TransactionModel> lista_mempool = new List<TransactionModel>();
            List<BlockModel> Received_blockchain = new List<BlockModel>();
            BlockServices.get_chain();
            MempoolServices.get_mempool();

            try {
                Received_blockchain = dados.ToObject<List<BlockModel>>();
                is_blockchain = true;
            } catch {
                try {
                    lista_mempool = dados.ToObject<List<TransactionModel>>();
                } catch {
                    BadRequest("Conteúdo Obtido Inválida");
                }
            }

            if (is_blockchain) {

                int lastIndexReceived = Received_blockchain.Count - 1;
                int lastIndexBlockServicesChain = BlockServices.chain.Count - 1;

                // Verificando se as Duas Blockchains são iguais
                bool Equal_blockchains = Received_blockchain[lastIndexReceived].hash == BlockServices.chain[lastIndexBlockServicesChain].hash &&
                    Received_blockchain[lastIndexReceived].index == BlockServices.chain[lastIndexBlockServicesChain].index;

                if (Equal_blockchains) {

                    int receivedConfirmations = Received_blockchain[lastIndexReceived].confirmations;
                    int blockServicesConfirmations = BlockServices.chain[lastIndexBlockServicesChain].confirmations;

                    // Atualizar confirmations se o valor recebido for maior
                    BlockServices.chain[lastIndexBlockServicesChain].confirmations = (receivedConfirmations > blockServicesConfirmations) ? receivedConfirmations : blockServicesConfirmations;
                    return Ok(BlockServices.get_chain());
                } else if (Received_blockchain[lastIndexReceived].index > BlockServices.chain[lastIndexBlockServicesChain].index &&
                    Received_blockchain[lastIndexReceived].previous_hash == BlockServices.chain[lastIndexBlockServicesChain].hash) {

                    // Atualizando a Blockchain
                    Received_blockchain[lastIndexReceived].confirmations += 1;
                    BlockServices.chain = Received_blockchain;
                    return Ok(BlockServices.get_chain());

                } else if (Received_blockchain[lastIndexReceived].index < BlockServices.chain[lastIndexBlockServicesChain].index) {

                    P2PMethors.SendBlockchain();
                    return Ok(BlockServices.get_chain());
                }

            } else if (!is_blockchain) {

                // Verificando se as Duas Mempool são iguais
                bool validation_1 = lista_mempool[lista_mempool.Count-1].timestamp ==
                MempoolServices.mempool[MempoolServices.mempool.Count-1].timestamp;
                bool validation_2 = lista_mempool[lista_mempool.Count-1].index ==
                MempoolServices.mempool[MempoolServices.mempool.Count-1].index;

                // Convertendo strings para DateTime
                DateTime dateTime1 = DateTime.ParseExact(lista_mempool[lista_mempool.Count-1].timestamp, "dd/MM/yyyy HH:mm:ss", null);
                DateTime dateTime2 = DateTime.ParseExact(MempoolServices.mempool[MempoolServices.mempool.Count-1].timestamp, "dd/MM/yyyy HH:mm:ss", null);

                if (validation_1 && validation_2) {

                    return Ok(MempoolServices.get_mempool());
                } else if (dateTime1 > dateTime2) {

                    // Atualizando a Mempool
                    MempoolServices.mempool = lista_mempool;
                    return Ok(MempoolServices.get_mempool());
                } else if (dateTime1 < dateTime2) {

                    // Enviando a Mempool Atualizada para os Nós
                    P2PMethors.SendMempool();
                    return Ok(MempoolServices.get_mempool());
                }
            }

            return BadRequest();
        }
    }
}