using Microsoft.AspNetCore.Mvc;
using BlockchainDemo.Models;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System;


namespace BlockchainDemo.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class P2PController : ControllerBase {

        public async void SendBlockchain() {

            var MainController = new MainController();

            for (int i = 0; i < MainController.node.Count; i++) {

                // Criar um objeto HttpClient
                using (HttpClient client = new HttpClient()) {

                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
                    );

                    string json = JsonConvert.SerializeObject(MainController.chain, Formatting.Indented);

                    // Enviar a solicitação POST
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    try {
                        HttpResponseMessage response = await client.PostAsync("http://" + MainController.node[i] + ":7000/P2P", content);
                    } catch {
                        
                    }
                }
            }
        }

        [HttpPost]
        public IActionResult ConnectionNode([FromBody] dynamic dadosObtidos) {

            var MainController = new MainController();

            // Salvando o IP do nó
            var ipAddress = HttpContext.Connection.RemoteIpAddress;

            // Verifica se o endereço IP é do tipo IPv6 e mapeia para IPv4, se necessário
            if (ipAddress != null && ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6) {
                ipAddress = ipAddress.MapToIPv4();
            }

            var ipv4Address = ipAddress.ToString();

            if (!MainController.node.Contains(ipv4Address)) {
                MainController.node.Add(ipv4Address);
            }

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

            if (lista_obtida.Count > MainController.chain.Count) {
                MainController.chain = lista_obtida;

                return Ok(MainController.get_chain());
            } else if (lista_obtida.Count < MainController.chain.Count) {
                SendBlockchain();

                return Ok(MainController.get_chain());
            }

            return Ok();
        }
    }
}