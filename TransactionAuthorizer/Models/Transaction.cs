namespace TransactionAuthorizer.Models
{
    public class Transaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid accountId { get; set; } = Guid.NewGuid();
        public string Account { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string MCC { get; set; } = string.Empty;
        public string Merchant { get; set; } = string.Empty;
    }
}
