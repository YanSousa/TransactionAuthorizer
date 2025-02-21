namespace TransactionAuthorizer.Services
{
    public interface IBalanceService
    {
        bool HasSufficientBalance(string category, decimal amount);
        bool DeductBalance(string category, decimal amount);
        decimal GetBalance(string category);
    }
}
