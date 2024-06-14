namespace BlockchainDemo.Models {

    public class BlockModel {

        public int index { get; set; }
        public int uBits { get; set; }
        public int nonce { get; set; }
        public int confirmations { get; set; }
        public long timestamp { get; set; }
        public string merkleRoot { get; set; }
        public string hash { get; set; }
        public string previous_hash { get; set; }
        public int txnCounter { get; set; }
        public List<TransactionModel> transactions { get; set; }

        public BlockModel() {
            index = 0;
            uBits = 0;
            nonce = 0;
            confirmations = 0;
            timestamp = 0;
            merkleRoot = string.Empty;
            hash = string.Empty;
            previous_hash = string.Empty;
            txnCounter = 0;
            transactions = [];
        }

    }
}