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
            List<TransactionModel> transactions = new List<TransactionModel>(MempoolServices.mempool.Take(num_max).ToList());

            if (MempoolServices.mempool.Count() == 0) {
                throw new Exception("Nenhuma Transação na Mempool");
            }

            int index = 0;

            foreach (var transaction in MempoolServices.mempool.Take(num_max).ToList()) {
                UserServices.get_user();

                var transactions_mine = new TransactionModel {
                    timestamp = DateTime.Now.ToString(),
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
        * Esta função Cria um Bloco que será inserido na Blockchain.
        * 
        * @param {string} previous_hash - Hash do bloco Anterior.
        * @param {List<TransactionModel>} list_transaction - Lista de Transações que será incluída no Bloco.
        * @returns {BlockModel} - Retorna o Bloco que vai fazer parte da Blockchain.
        */
        public BlockModel create_block(string previous_hash, List<TransactionModel> list_transaction) {

            var MainServices = new MainServices();

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
                string calculatedHash = MainServices.CalculateSHA256Hash(blockJson);
                block.hash = calculatedHash;

                block.nonce += 1;
            }

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

            string caminhoArquivo = "Database/blockchain.hex";
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
