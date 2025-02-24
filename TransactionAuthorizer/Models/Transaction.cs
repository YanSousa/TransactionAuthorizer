namespace TransactionAuthorizer.Models
{
    public class Transaction
    {
        public string Account { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string MCC { get; set; } = string.Empty;
        public string Merchant { get; set; } = string.Empty;
    }
}
