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

    public class UserServices{

        public static UserModel user = new UserModel();
    
        /*
        * 
        * Esta função responsável pela Criação de uma Chave Privada,
        * uma Pública e Endereços Ligados a essas Chaves.
        *
        * @returns {void}
        */
        private void Create_user() {

            var MainServices = new MainServices();

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
                    string calculatedHash = MainServices.CalculateSHA256Hash(transactionJson);
                    user.address.Add(calculatedHash);

                }
            }

            user.index = 0;
        }

        /*
        * Esta função é responsável por obter a Usuário do Arquivo user.hex ou da váriavel user.
        * 
        * @returns {UserModel} - Retorna o Usuário.
        */
        public UserModel get_user() {

            var MainServices = new MainServices();

            string caminhoArquivo = "Database/user.hex";
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
                    user = MainServices.ConvertHexadecimalToList(conteudoHexadecimal);
                } catch {
                    Create_user();
                }
            }

            // Convertendo lista para uma representação Hexadecimal
            byte[] bytes = MainServices.ConvertListToHexadecimal(user);
            string hex = BitConverter.ToString(bytes).Replace("-", "");

            // Salvando a representação hexadecimal no arquivo
            using (StreamWriter sw = new StreamWriter(caminhoArquivo)) {
                sw.WriteLine(hex);
            }

            return user;
        }
    }
}