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

    public class MainServices {

        /*
        * Esta função Transformar o input em um Hash SHA256.
        * 
        * @param {string} input - Dados recebidos que serão convertidos em SHA256.
        * @returns {string} - Retorna o SHA256.
        */
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

        /*
        * Esta função Transformar a Lista em Hexadecimal.
        * 
        * @param {dynamic} lista - Dados recebidos que serão convertidos em Hexadecimal.
        * @returns {byte[]} - Retorna o Hexadecimal.
        */
        public static byte[] ConvertListToHexadecimal(dynamic lista) {
            string json = JsonConvert.SerializeObject(lista);
            return Encoding.UTF8.GetBytes(json);
        }

        /*
        * Esta função Transformar o Hexadecimal recebido em uma Lista de Blocos ou em UserModel.
        * 
        * @param {string} hex - Dados recebidos que serão convertidos em Hexadecimal.
        * @returns {dynamic} - Retorna a Lista ou UserModel.
        */
        public static dynamic ConvertHexadecimalToList(string hex) {

            var UserServices = new UserServices();

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
                return JsonConvert.DeserializeObject<UserModel>(json) ?? UserServices.user;
            }
        }
    }
}