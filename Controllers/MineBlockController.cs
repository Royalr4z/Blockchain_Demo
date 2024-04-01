using Microsoft.AspNetCore.Mvc;
using BlockchainDemo.Models;
using BlockchainDemo.Config;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Text;
using System;


namespace BlockchainDemo.Controllers {

    [Route("[controller]")]
    [ApiController]

    public class MineBlockController : ControllerBase {

        private List<TransactionModel> mine_block(dynamic dadosObtidos) {
    
            // Convertendo os Dados Obtidos para JSON
            string jsonString = System.Text.Json.JsonSerializer.Serialize(dadosObtidos);
            dynamic dados = JsonConvert.DeserializeObject<dynamic>(jsonString);

            List<TransactionModel> lista_t = dados["transactions"].ToObject<List<TransactionModel>>();
            List<TransactionModel> transactions = new List<TransactionModel>();

            Validate validator = new Validate();

            int index = 1;

            foreach (var item in lista_t) {

                validator.existsOrError(item.from, @"Informe o remetente - Index: " + index);
                validator.existsOrError(item.towards, @"Informe o destinatário - Index: " + index);

                validator.existsDecimalOrError(item.value, @"Informe o valor da Transação");
                validator.existsDecimalOrError(item.rate, @"Informe o valor da Taxa");

                var BlockController = new BlockController();

                item.timestamp = DateTime.Now.ToString();
                item.index = index;

                // Serializar o bloco para uma string JSON
                string transactionJson = JsonConvert.SerializeObject(item);
                string calculatedHash = BlockController.CalculateSHA256Hash(transactionJson);
                item.id_transaction = calculatedHash;

                transactions.Add(item);
                index += 1;
            }

            return transactions;
        }


        [HttpPost]
        public ActionResult<List<TransactionModel>> post_block([FromBody] dynamic dadosObtidos) {

            var BlockController = new BlockController();

            BlockController.get_chain();
            BlockController.create_block(BlockController.chain[BlockController.chain.Count - 1].hash, mine_block(dadosObtidos));

            try {

                return Ok(BlockController.get_chain());
            } catch (Exception ex) {

                return BadRequest(ex.Message);
            }
        }

    }
}


//
//
//        ### Json Esperado ###
//
//        {
//            "transactions": [
//                  { "id_transaction": "agea45gg", "from": "egqegq", "towards": "para", "value": 0.2, "rate": 0.01 },
//                  { "id_transaction": "vqrvq214", "from": "egqegq", "towards": "para", "value": 0.7, "rate": 0.01 },
//                  { "id_transaction": "623wbrw", "from": "egqegq", "towards": "para", "value": 0.01, "rate": 0.001 },
//                  { "id_transaction": "7a5vwe", "from": "egqegq", "towards": "para", "value": 0.5, "rate": 0.01 },
//                  { "id_transaction": "bqrbq87", "from": "egqegq", "towards": "para", "value": 0.1, "rate": 0.01 },
//                  { "id_transaction": "qbrqb89", "from": "egqegq", "towards": "para", "value": 0.4, "rate": 0.01 }
//            ]
//        }
//
//
