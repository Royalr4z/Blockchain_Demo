namespace BlockchainDemo.Models {

    public class UserModel {

        public int index { get; set; }
        public string private_key { get; set; }
        public string public_key { get; set; }
        public List<string> address { get; set; }

        public UserModel() {
            index = 0;
            private_key = string.Empty;
            public_key = string.Empty;
            address = [];
        }

    }
}