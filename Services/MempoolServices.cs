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

    public class MempoolServices {

        public static List<TransactionModel> mempool = new List<TransactionModel>();

        /*
        * Esta função processa os dados enviados pelo método POST,
        * realiza validações e adiciona na mempool as transações
        * 
        * @param {dynamic} dadosObtidos - Dados recebidos do método POST.
        * @returns {void}
        */
        public void add_transaction(dynamic dadosObtidos) {

            string caminhoArquivo = "Database/mempool.hex";

            var MainServices = new MainServices();
            
            // Convertendo os Dados Obtidos para JSON
            string jsonString = System.Text.Json.JsonSerializer.Serialize(dadosObtidos);
            dynamic? dados = JsonConvert.DeserializeObject<dynamic>(jsonString);

            // Obtendo a Lista de Transações
            List<TransactionModel> lista_t;

            try {
                lista_t = dados["transactions"].ToObject<List<TransactionModel>>();
            } catch {
                throw new Exception("Nenhuma Transação Enviada");
            }

            Validate validator = new Validate();
            int index;

            if (mempool.Count() == 0) {
                index = 0;
            } else { 
                index = mempool.Last().index + 1;
            }

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
                string calculatedHash = MainServices.CalculateSHA256Hash(transactionJson);
                item.id_transaction = calculatedHash;

                mempool.Add(item);
                index += 1;
            }

            // Convertendo lista para uma representação Hexadecimal
            byte[] bytes = MainServices.ConvertListToHexadecimal(mempool);
            string hex = BitConverter.ToString(bytes).Replace("-", "");

            // Salvando a representação hexadecimal no arquivo
            using (StreamWriter sw = new StreamWriter(caminhoArquivo)) {
                sw.WriteLine(hex);
            }
        }

        /*
        * Esta função é responsável por obter a Mempool do Arquivo mempool.hex ou da váriavel mempool.
        * 
        * @returns {List<TransactionModel>} - Retorna a Mempool.
        */
        public List<TransactionModel> get_mempool() {

            var MainServices = new MainServices();

            string pastaDoArquivo = "Database";
            string caminhoArquivo = pastaDoArquivo + "/mempool.hex";
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

            try {
                // Convertendo o Hexadecimal para uma Lista de Blocos
                mempool = MainServices.ConvertHexadecimalToTransactions(conteudoHexadecimal);
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }

            return mempool;
        }
    }
}