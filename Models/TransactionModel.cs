namespace BlockchainDemo.Models {

    public class TransactionModel {

        public int index { get; set; }
        public string id_transaction { get; set; }
        public string timestamp { get; set; }
        public string from { get; set; }
        public string towards { get; set; }
        public decimal value { get; set; }
        public decimal rate { get; set; }

        public TransactionModel() {
            index = 0;
            id_transaction = string.Empty;
            from = string.Empty;
            towards = string.Empty;
            value = 0;
            rate = 0;
        }

    }
}