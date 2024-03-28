using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlockchainDemo.Config {

    public class Validate {

        public void existsOrError(string value, string msg) {
    
            if(string.IsNullOrEmpty(value)) {
                throw new Exception(msg);
            }

            // Se value for uma string e estiver vazia após remover espaços em branco
            if (value is string && string.IsNullOrWhiteSpace((string)value)) {
                throw new Exception(msg);
            }

        }

        public void existsDecimalOrError(decimal value, string msg) {
    
            if (value == 0) {
                throw new Exception(msg);
            }
        }

        public void notExistsOrError(string value, string msg) {
            try {
                existsOrError(value, msg);
            } catch(Exception) {
                return;
            }
            throw new Exception(msg);
        }

        public void equalsOrError(string valueA, string valueB, string msg) {
            if (valueA != valueB) {
                throw new Exception(msg);
            }
        }

    }
}