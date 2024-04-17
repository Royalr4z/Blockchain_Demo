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
            dynamic? dados = JsonConvert.DeserializeObject<dynamic>(jsonString);

            // Obtendo a Lista de Transações
            List<TransactionModel> lista_t;
            List<TransactionModel> transactions = new List<TransactionModel>();

            try {
                lista_t = dados["transactions"].ToObject<List<TransactionModel>>();
            } catch {
                throw new Exception("Nenhuma Transação Enviada");
            }

            Validate validator = new Validate();

            int index = 0;

            if (lista_t.Count() == 0) {
                throw new Exception("Nenhuma Transação Enviada");
            }

            foreach (var item in lista_t) {

                validator.existsOrError(item.from, @"Informe o remetente - Index: " + index);
                validator.existsOrError(item.towards, @"Informe o destinatário - Index: " + index);

                validator.existsDecimalOrError(item.value, @"Informe o valor da Transação - Index: " + index );
                validator.existsDecimalOrError(item.rate, @"Informe o valor da Taxa - Index: " + index);

                item.timestamp = DateTime.Now.ToString();
                item.index = index;

                // Criação de um Hash Único para a Transação (id_transaction)
                string transactionJson = JsonConvert.SerializeObject(item);
                string calculatedHash = CalculateSHA256Hash(transactionJson);
                item.id_transaction = calculatedHash;

                transactions.Add(item);
                index += 1;
            }

            List<TransactionModel> list_mine = new List<TransactionModel>();

            foreach (var transaction in transactions) {
                get_user();

                var transactions_mine = new TransactionModel {
                    timestamp = DateTime.Now.ToString(),
                    index = index,
                    from = transaction.from,
                    towards = user.address[0],
                    value = transaction.rate,
                    rate = 0
                };

                // Criação de um Hash Único para a Transação (id_transaction)
                string transactionJson = JsonConvert.SerializeObject(transactions_mine);
                string calculatedHash = CalculateSHA256Hash(transactionJson);
                transactions_mine.id_transaction = calculatedHash;

                list_mine.Add(transactions_mine);
                index += 1;
            }
            transactions.AddRange(list_mine);

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

        public static byte[] ConvertListToHexadecimal(dynamic lista) {
            string json = JsonConvert.SerializeObject(lista);
            return Encoding.UTF8.GetBytes(json);
        }

        public static dynamic ConvertHexadecimalToList(string hex) {

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
            try {
                return JsonConvert.DeserializeObject<List<BlockModel>>(json) ?? [];
            } catch {
                return JsonConvert.DeserializeObject<UserModel>(json) ?? user;
            }
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

        public static UserModel user = new UserModel();

        private void Create_user() {

            using (ECDsa ecdsa = ECDsa.Create()) {

                // Gera a chave privada
                byte[] privateKey = ecdsa.ExportECPrivateKey();

                // Gera a chave pública a partir da chave privada
                byte[] publicKey = ecdsa.ExportSubjectPublicKeyInfo();

                user.private_key = BitConverter.ToString(privateKey).Replace("-", "");
                user.public_key = BitConverter.ToString(publicKey).Replace("-", "");

                for (int i = 0; i < 15; i++) {
                    user.index = i;

                    // Criação dos Endereços
                    string transactionJson = JsonConvert.SerializeObject(user);
                    string calculatedHash = CalculateSHA256Hash(transactionJson);
                    user.address.Add(calculatedHash);

                }
            }

            user.index = 0;
        }

        public UserModel get_user() {

            string caminhoArquivo = "user.hex";
            string conteudoHexadecimal = "";

            if (File.Exists(caminhoArquivo)) {

                // Lê o conteúdo hexadecimal do arquivo
                using (StreamReader sr = new StreamReader(caminhoArquivo)) {
                    conteudoHexadecimal = sr.ReadToEnd();
                }
            }

            // Retirando os Espaços em Branco
            conteudoHexadecimal = conteudoHexadecimal.Replace(" ", "").Trim();

            if (user.address.Count == 0 && conteudoHexadecimal == "") {
                Create_user();
            } else if (user.address.Count == 0 && conteudoHexadecimal != "") {

                try {
                    // Convertendo o Hexadecimal em Informações do Usuário
                    user = ConvertHexadecimalToList(conteudoHexadecimal);
                } catch {
                    Create_user();
                }
            }

            // Convertendo lista para uma representação Hexadecimal
            byte[] bytes = ConvertListToHexadecimal(user);
            string hex = BitConverter.ToString(bytes).Replace("-", "");

            // Salvando a representação hexadecimal no arquivo
            using (StreamWriter sw = new StreamWriter(caminhoArquivo)) {
                sw.WriteLine(hex);
            }

            return user;
        }
    }
}
