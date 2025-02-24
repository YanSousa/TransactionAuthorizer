using TransactionAuthorizer.Models;

namespace TransactionAuthorizer.Services
{
    public interface IBalanceService
    {
        bool DeductBalance(string account, string category, decimal amount);
        decimal GetBalance(UserAccount user, string category);
        void SetBalance(UserAccount user, string category, decimal newBalance);
    }
}
