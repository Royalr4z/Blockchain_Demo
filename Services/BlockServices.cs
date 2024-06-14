using Microsoft.AspNetCore.Mvc;
using BlockchainDemo.Models;
using BlockchainDemo.Config;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System;


namespace BlockchainDemo.Services {

    public class BlockServices {

        public static List<BlockModel> chain = new List<BlockModel>();
        public static string merkleRoot = "0000000000000000000000000000000000000000000000000000000000000000";

        /*
        * Esta função Retorna todas as transações que serão inseridas no Bloco, Incluindo as Transações das taxas
        * para os Mineradores do Bloco.
        * 
        * @returns {List<TransactionModel>} - Uma lista de transações pronta para ser inserida em um bloco.
        */
        public List<TransactionModel> mine_block() {

            string caminhoArquivo = "Database/mempool.hex";

            var MainServices = new MainServices();
            var UserServices = new UserServices();
            var MempoolServices = new MempoolServices();

            // Número máximo de Transações por Bloco
            int num_max = 1000;

            MempoolServices.get_mempool();
            List<TransactionModel> list_mine = new List<TransactionModel>();
            List<TransactionModel> transactions = new List<TransactionModel>(MempoolServices.mempool.Skip(1).Take(num_max).ToList());

            if (transactions.Count() == 0) {
                throw new Exception("Nenhuma Transação na Mempool");
            }

            int index = 0;

            foreach (var transaction in MempoolServices.mempool.Skip(1).Take(num_max).ToList()) {
                UserServices.get_user();

                var transactions_mine = new TransactionModel {
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    index = index,
                    from = transaction.from,
                    towards = UserServices.user.address[0],
                    value = transaction.rate,
                    rate = 0
                };

                // Criação de um Hash Único para a Transação (id_transaction)
                string transactionJson = JsonConvert.SerializeObject(transactions_mine);
                string calculatedHash = MainServices.CalculateSHA256Hash(transactionJson);
                transactions_mine.id_transaction = calculatedHash;

                list_mine.Add(transactions_mine);
                index += 1;
            }
            transactions.AddRange(list_mine);

            MempoolServices.mempool.RemoveAll(transactions.Contains);

            // Convertendo lista para uma representação Hexadecimal
            byte[] bytes = MainServices.ConvertListToHexadecimal(MempoolServices.mempool);
            string hex = BitConverter.ToString(bytes).Replace("-", "");

            // Salvando a representação hexadecimal no arquivo
            using (StreamWriter sw = new StreamWriter(caminhoArquivo)) {
                sw.WriteLine(hex);
            }

            return transactions;
        }


        /*
        * Esta função Converte string em um array de Bytes.
        * 
        * @param {string} hex - Hash que vai ser convertido.
        * @returns {void}
        */
        private byte[] convertStringByArray(string hex) {

            int length = hex.Length;
            byte[] bytes = new byte[length / 2];
            for (int i = 0; i < length; i += 2) {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }


        /*
        * Esta função soma o hash da primeira transação com o da segunda e, em seguida, adiciona o
        * resultado ao merkleRoot para, no final, obter o hash Merkle do bloco.
        * 
        * @param {string} hash_tnx1 - Hash da Transação N° 1.
        * @param {string} hash_tnx2 - Hash da Transação N° 2.
        * @returns {void}
        */
        private void merkle_tree(string hash_tnx1, string hash_tnx2) {

            byte[] hash1 = convertStringByArray(hash_tnx1);
            byte[] hash2 = convertStringByArray(hash_tnx2);
            byte[] hash_merkleRoot = convertStringByArray(merkleRoot);

            if (hash1.Length != hash2.Length)
            throw new ArgumentException("Hashes must be of the same length");

            byte[] result = new byte[hash1.Length];

            for (int i = 0; i < hash1.Length; i++){
                // Somar os valores byte a byte com overflow
                result[i] = (byte)((hash_merkleRoot[i] + (hash1[i] + hash2[i])) % 256);
            }

            merkleRoot = BitConverter.ToString(result).Replace("-", "").ToLower();

        }


        /*
        * Esta função Cria um Bloco que será inserido na Blockchain.
        * 
        * @param {string} previous_hash - Hash do bloco Anterior.
        * @param {List<TransactionModel>} list_transaction - Lista de Transações que será incluída no Bloco.
        * @returns {BlockModel} - Retorna o Bloco que vai fazer parte da Blockchain.
        */
        public BlockModel create_block(string previous_hash, List<TransactionModel> list_transaction) {

            var MainServices = new MainServices();
            int num_uBits = 5;

            for (int i = 0; i < list_transaction.Count; i += 2) {
                merkle_tree(list_transaction[i].id_transaction, list_transaction[i+1].id_transaction);
            }

            var block = new BlockModel() {
                index = chain.Count,
                uBits = num_uBits,
                nonce = 0,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                merkleRoot = merkleRoot,
                hash = "",
                previous_hash = previous_hash,
                txnCounter = list_transaction.Count(),
            };

            merkleRoot = "0000000000000000000000000000000000000000000000000000000000000000";

            // Criação do Hash do Bloco
            block.hash = MainServices.CalculateSHA256Hash(JsonConvert.SerializeObject(block));

            // Verificando se o Hash está de acordo com a dificuldade estabelecida (uBits)
            while (block.hash.Substring(0, num_uBits) !=  String.Concat(Enumerable.Repeat("0", num_uBits))) {

                block.nonce += 1;
                block.hash = "";
                block.timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                // Serializar o bloco para uma string JSON
                string blockJson = JsonConvert.SerializeObject(block);
                string calculatedHash = MainServices.CalculateSHA256Hash(blockJson);
                block.hash = calculatedHash;

            }

            block.transactions = list_transaction;
            block.confirmations = 1;
            chain.Add(block);

            return block;
        }

        /*
        * Esta função é responsável por obter a Blockhain do Arquivo blockchain.hex ou da váriavel Chain.
        * 
        * @returns {List<BlockModel>} - Retorna a Blockhain.
        */
        public List<BlockModel> get_chain() {

            var MainServices = new MainServices();

            string pastaDoArquivo = "Database";
            string caminhoArquivo = pastaDoArquivo + "/blockchain.hex";
            string conteudoHexadecimal = "";

            // Verifica se a pasta não existe antes de tentar criá-la
            if (!Directory.Exists(pastaDoArquivo)) {
                // Cria a pasta
                Directory.CreateDirectory(pastaDoArquivo);
            }

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
                    chain = MainServices.ConvertHexadecimalToList(conteudoHexadecimal);
                } catch {
                    // Caso o conteúdo Hexadecimal seja Inválido, Criar um Novo Bloco Gênesis
                    create_block("0", []);
                }
            }

            // Convertendo lista para uma representação Hexadecimal
            byte[] bytes = MainServices.ConvertListToHexadecimal(chain);
            string hex = BitConverter.ToString(bytes).Replace("-", "");

            // Salvando a representação hexadecimal no arquivo
            using (StreamWriter sw = new StreamWriter(caminhoArquivo)) {
                sw.WriteLine(hex);
            }

            return chain;
        }
    }
}
