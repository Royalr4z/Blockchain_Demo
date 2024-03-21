namespace BlockchainDemo.Models {

    public class BlockModel {

        public int Nonce { get; set; }
        public string timestamp { get; set; }
        public string hash { get; set; }
        public string previous_hash { get; set; }

        public BlockModel(dynamic transactions) {
            Nonce = 0;
            timestamp = string.Empty;
            hash = string.Empty;
            previous_hash = string.Empty;
        }


    }
}