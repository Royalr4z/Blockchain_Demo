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
        * Esta função Envia via TCP/IP o último Bloco Como um Hexadecimal para todos os IPs da Variável node.
        * 
        * @returns {void}
        */
        public void SendBlock() {

            UpdateIPS();

            var BlockServices = new BlockServices();
            var MainServices = new MainServices();

            for (int i = 0; i < node.Count; i++) {

                int serverPort = 7001;
                // Convertendo lista para uma representação Hexadecimal
                int lastIndex = BlockServices.chain.Count() - 1;
                byte[] hex = MainServices.ConvertListToHexadecimal(BlockServices.chain[lastIndex]);

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
            var MainServices = new MainServices();
            var P2PMethors = new P2PMethors();
            bool is_block = false;

            // Convertendo os Dados Obtidos para JSON
            string jsonString = System.Text.Json.JsonSerializer.Serialize(dadosObtidos);
            dynamic? dados = JsonConvert.DeserializeObject<dynamic>(jsonString);

            // Obtendo a Lista de Transações ou a Lista de Blocos
            List<TransactionModel> Received_mempool = new List<TransactionModel>();
            var Received_block = new BlockModel();
            BlockServices.get_chain();
            MempoolServices.get_mempool();

            try {
                Received_block = dados.ToObject<BlockModel>();
                is_block = true;
            } catch {
                try {
                    Received_mempool = dados.ToObject<List<TransactionModel>>();
                } catch {
                    BadRequest("Conteúdo Obtido Inválida");
                }
            }

            if (is_block) {

                int lastIndexBlockchain = BlockServices.chain.Count - 1;

                // Verificando se as Duas Blockchains são iguais
                bool Equal_blockchains = Received_block.hash == BlockServices.chain[lastIndexBlockchain].hash &&
                    Received_block.index == BlockServices.chain[lastIndexBlockchain].index;

                if (Equal_blockchains) {

                    int receivedConfirmations = Received_block.confirmations;
                    int blockServicesConfirmations = BlockServices.chain[lastIndexBlockchain].confirmations;

                    // Atualizar confirmations se o valor recebido for maior
                    BlockServices.chain[lastIndexBlockchain].confirmations = (receivedConfirmations > blockServicesConfirmations) ? receivedConfirmations : blockServicesConfirmations;
                    return Ok(BlockServices.get_chain());
                } else if (Received_block.index > BlockServices.chain[lastIndexBlockchain].index &&
                    Received_block.previous_hash == BlockServices.chain[lastIndexBlockchain].hash) {

                    // Atualizando a Blockchain
                    Received_block.confirmations += 1;
                    BlockServices.chain.Add(Received_block);

                    foreach (var Received_txn in Received_block.transactions) {
                        // Removendo as Transações que foi inserida na blockchain
                        MempoolServices.mempool.RemoveAll(txn => txn.id_transaction == Received_txn.id_transaction);
                    }

                    // Convertendo lista para uma representação Hexadecimal
                    byte[] bytes = MainServices.ConvertListToHexadecimal(MempoolServices.mempool);
                    string hex = BitConverter.ToString(bytes).Replace("-", "");

                    // Salvando a representação hexadecimal no arquivo
                    using (StreamWriter sw = new StreamWriter("Database/mempool.hex")) {
                        sw.WriteLine(hex);
                    }

                    return Ok(BlockServices.get_chain());

                } else if (Received_block.index < BlockServices.chain[lastIndexBlockchain].index) {

                    P2PMethors.SendBlock();
                    return Ok(BlockServices.get_chain());
                }

            } else if (!is_block) {

                // Verificando se as Duas Mempool são iguais
                bool Equal_mempool = Received_mempool[Received_mempool.Count-1].timestamp == MempoolServices.mempool[MempoolServices.mempool.Count-1].timestamp &&
                    Received_mempool[Received_mempool.Count-1].index == MempoolServices.mempool[MempoolServices.mempool.Count-1].index;

                // Obtendo o timestamp da mempool recebida e da mempool local
                long dateTime_received = Received_mempool[0].timestamp;
                long dateTime_local = MempoolServices.mempool[0].timestamp;

                if (Equal_mempool) {

                    return Ok(MempoolServices.get_mempool());
                } else if (dateTime_received > dateTime_local) {

                    // Atualizando a Mempool
                    MempoolServices.mempool = Received_mempool;
                    return Ok(MempoolServices.get_mempool());
                } else if (dateTime_received < dateTime_local) {

                    // Enviando a Mempool Atualizada para os Nós
                    P2PMethors.SendMempool();
                    return Ok(MempoolServices.get_mempool());
                }
            }

            return BadRequest();
        }
    }
}