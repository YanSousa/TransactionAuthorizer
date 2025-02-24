namespace TransactionAuthorizer.Models
{
    public class UserAccount
    {
        public string Account { get; set; } = string.Empty;
        public decimal FoodBalance { get; set; }
        public decimal MealBalance { get; set; }
        public decimal CashBalance { get; set; }
    }
}
