using System.Collections.Concurrent;

namespace TransactionAuthorizer.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly ConcurrentDictionary<string, decimal> _balances = new()
        {
            ["FOOD"] = 500.00m,
            ["MEAL"] = 300.00m,
            ["CASH"] = 1000.00m
        };

        public bool HasSufficientBalance(string category, decimal amount)
        {
            return _balances.ContainsKey(category) && _balances[category] >= amount;
        }

        public bool DeductBalance(string category, decimal amount)
        {
            if (!HasSufficientBalance(category, amount)) return false;

            _balances[category] -= amount;
            return true;
        }

        public decimal GetBalance(string category)
        {
            return _balances.ContainsKey(category) ? _balances[category] : 0;
        }
    }
}
