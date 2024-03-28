using Microsoft.AspNetCore.Mvc;
using BlockchainDemo.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using System;


namespace BlockchainDemo.Controllers {

    [Route("[controller]")]
    [ApiController]

    public class BlockController : ControllerBase {
    
        List<BlockModel> chain = [];

        private BlockModel create_block(int proof, string previous_hash) {

            List<TransactionModel> list_transaction = [];

            var transactions = new TransactionModel() {
                index = 0,
                id_transaction = string.Empty,
                from = string.Empty,
                towards = string.Empty,
                value = 0,
                rate = 0
            };

            list_transaction.Add(transactions);

            var block = new BlockModel() {
                index = chain.Count,
                nonce = proof,
                timestamp = DateTime.Now.ToString(),
                transactions = list_transaction,
                hash = "hash_teste",
                previous_hash = previous_hash,
            };

            chain.Add(block);

            return block;
        }

        public static byte[] ConvertListToHexadecimal(List<BlockModel> lista) {
            string json = JsonConvert.SerializeObject(lista);
            return Encoding.UTF8.GetBytes(json);
        }

        private List<BlockModel> get_chain() {

            if (chain.Count == 0) {
                create_block(1, "0");
            }
            
            // Convertendo lista para uma representação binária
            byte[] bytes = ConvertListToHexadecimal(chain);

            Console.WriteLine("Lista em binário: " + BitConverter.ToString(bytes).Replace("-", ""));

            return chain;
        }

        [HttpGet]
        public ActionResult<List<BlockModel>> Get_blockchain() {

            return Ok(get_chain());
        }

    }

}