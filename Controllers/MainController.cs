using Microsoft.AspNetCore.Mvc;
using BlockchainDemo.Models;
using BlockchainDemo.Config;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System;


namespace BlockchainDemo.Controllers {
    public class MainController {

        public List<TransactionModel> mine_block(dynamic dadosObtidos) {
    
            // Convertendo os Dados Obtidos para JSON
            string jsonString = System.Text.Json.JsonSerializer.Serialize(dadosObtidos);
            dynamic dados = JsonConvert.DeserializeObject<dynamic>(jsonString);

            // Obtendo a Lista de Transações
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

                // Criação de um Hash Único para a Transação (id_transaction)
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

        private static List<BlockModel> ConvertHexadecimalToList(string hex) {

            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];

            for (int i = 0; i < numberChars; i += 2) {
                // Verifica se há caracteres suficientes para formar uma substring de dois caracteres
                if (i + 1 < numberChars) { 
                    string substring = hex.Substring(i, 2);

                    bytes[i / 2] = Convert.ToByte(substring, 16);
                }
            }

            string json = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<List<BlockModel>>(json);
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

            string caminhoArquivo = "blockchain.hex";
            string conteudoHexadecimal = "";

            if (File.Exists(caminhoArquivo)) {

                // Lê o conteúdo hexadecimal do arquivo
                using (StreamReader sr = new StreamReader(caminhoArquivo)) {
                    conteudoHexadecimal = sr.ReadToEnd();
                }
            }

            // Retirando os Espaços em Branco
            conteudoHexadecimal = conteudoHexadecimal.Replace(" ", "").Trim();

            if (chain.Count == 0 && conteudoHexadecimal == "") {
                create_block("0", []);
            } else if (chain.Count == 0 && conteudoHexadecimal != "") {

                try {
                    // Convertendo o Hexadecimal para uma Lista de Blocos
                    chain = ConvertHexadecimalToList(conteudoHexadecimal);
                } catch {
                    // Caso o conteúdo Hexadecimal seja Inválido, Criar um Novo Bloco Gênesis
                    create_block("0", []);
                }
            }

            // Convertendo lista para uma representação Hexadecimal
            byte[] bytes = ConvertListToHexadecimal(chain);
            string hex = BitConverter.ToString(bytes).Replace("-", "");

            // Salvando a representação hexadecimal no arquivo
            using (StreamWriter sw = new StreamWriter(caminhoArquivo)) {
                sw.WriteLine(hex);
            }

            return chain;
        }
    }
}
