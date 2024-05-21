namespace BlockchainDemo.Models {

    public class BlockModel {

        public int index { get; set; }
        public int nonce { get; set; }
        public int uBits { get; set; }
        public string timestamp { get; set; }
        public List<TransactionModel> transactions { get; set; }
        public string hash { get; set; }
        public string previous_hash { get; set; }

        public BlockModel() {
            index = 0;
            nonce = 0;
            uBits = 0;
            timestamp = string.Empty;
            transactions = [];
            hash = string.Empty;
            previous_hash = string.Empty;
        }

    }
}