using Microsoft.AspNetCore.Mvc;
using BlockchainDemo.Models;
using BlockchainDemo.Config;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Text;
using System;


namespace BlockchainDemo.Controllers {
    public class MainController : ControllerBase {

        public List<TransactionModel> mine_block(dynamic dadosObtidos) {
    
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

                item.timestamp = DateTime.Now.ToString();
                item.index = index;

                // Serializar o bloco para uma string JSON
                string transactionJson = JsonConvert.SerializeObject(item);
                string calculatedHash = CalculateSHA256Hash(transactionJson);
                item.id_transaction = calculatedHash;

                transactions.Add(item);
                index += 1;
            }

            return transactions;
        }

        public static List<BlockModel> chain = new List<BlockModel>();

        public string CalculateSHA256Hash(string input) {

            using (SHA256 sha256 = SHA256.Create()) {

                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++) {
        
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private static byte[] ConvertListToHexadecimal(List<BlockModel> lista) {
            string json = JsonConvert.SerializeObject(lista);
            return Encoding.UTF8.GetBytes(json);
        }

        public BlockModel create_block(string previous_hash, List<TransactionModel> list_transaction) {

            var block = new BlockModel() {
                index = chain.Count,
                nonce = 1,
                timestamp = DateTime.Now.ToString(),
                transactions = list_transaction,
                hash = "0001",
                previous_hash = previous_hash,
            };

            while (block.hash.Substring(0, 4) != "0000") {

                // Serializar o bloco para uma string JSON
                string blockJson = JsonConvert.SerializeObject(block);
                string calculatedHash = CalculateSHA256Hash(blockJson);
                block.hash = calculatedHash;

                block.nonce += 1;
            }

            chain.Add(block);

            return block;
        }

        public List<BlockModel> get_chain() {

            if (chain.Count == 0) {
                create_block("0", []);
            }

            // Convertendo lista para uma representação Hexadecimal
            byte[] bytes = ConvertListToHexadecimal(chain);

            // Console.WriteLine("Lista em Hexadecimal: " + BitConverter.ToString(bytes).Replace("-", ""));

            return chain;
        }
    }
}
