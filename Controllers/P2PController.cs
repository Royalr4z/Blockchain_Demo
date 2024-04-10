using Microsoft.AspNetCore.Mvc;
using BlockchainDemo.Models;
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

        public void UpdateIPS() {

            if (!File.Exists(filePath)) {
                File.Create(filePath).Close();
            }

            node.AddRange(File.ReadAllLines(filePath));

        }

        public void SendBlockchain() {

            UpdateIPS();

            var MainController = new MainController();

            for (int i = 0; i < node.Count; i++) {

                int serverPort = 7001;
                // Convertendo lista para uma representação Hexadecimal
                byte[] hex = MainController.ConvertListToHexadecimal(MainController.chain);

                try {
                    // Criar uma instância TcpClient e se conecta ao servidor
                    using (TcpClient client = new TcpClient(node[i], serverPort)) {
                        // Obtém o stream de rede associado ao TcpClient
                        NetworkStream stream = client.GetStream();
                        // Enviar o Hexadecimal
                        stream.Write(hex, 0, hex.Length);
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

            var MainController = new MainController();
            var P2PMethors = new P2PMethors();

            // Convertendo os Dados Obtidos para JSON
            string jsonString = System.Text.Json.JsonSerializer.Serialize(dadosObtidos);
            dynamic? dados = JsonConvert.DeserializeObject<dynamic>(jsonString);

            // Obtendo a Lista de Transações
            List<BlockModel> lista_obtida = [];
            MainController.get_chain();

            try {
                lista_obtida = dados.ToObject<List<BlockModel>>();
            } catch {
                BadRequest("Blockchain Inválida");
            }

            // Verificando se as Duas Blockchains são iguais
            bool validation_1 = lista_obtida[lista_obtida.Count-1].hash ==
            MainController.chain[MainController.chain.Count-1].hash;
            bool validation_2 = lista_obtida[lista_obtida.Count-1].index ==
            MainController.chain[MainController.chain.Count-1].index;

            if (validation_1 && validation_2) {

                return Ok(MainController.get_chain());
            } else if (lista_obtida.Count > MainController.chain.Count) {

                // Atualizando a Blockchain
                MainController.chain = lista_obtida;
                return Ok(MainController.get_chain());
            } else if (lista_obtida.Count < MainController.chain.Count) {

                // Enviando a Blockchain Atualizada para os Nós
                P2PMethors.SendBlockchain();
                return Ok(MainController.get_chain());
            }

            return BadRequest();
        }
    }
}