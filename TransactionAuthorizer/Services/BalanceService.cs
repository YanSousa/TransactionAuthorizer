using System.Collections.Concurrent;
using TransactionAuthorizer.Models;
using TransactionAuthorizer.Utils;

namespace TransactionAuthorizer.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly IUserRepositoryService _userRepository;

        public BalanceService(IUserRepositoryService userRepository)
        {
            _userRepository = userRepository;
        }

        public bool DeductBalance(string account, string category, decimal amount)
        {
            var user = _userRepository.GetUser(account);
            if (user == null)
                return false;

            decimal balance = GetBalance(user, category);

            if (balance < amount)
                return false;

            SetBalance(user, category, balance - amount);
            return true;
        }

        public decimal GetBalance(UserAccount user, string category)
        {
            return category switch
            {
                TransactionCategories.Food => user.FoodBalance,
                TransactionCategories.Meal => user.MealBalance,
                _ => user.CashBalance
            };
        }

        public void SetBalance(UserAccount user, string category, decimal newBalance)
        {
            switch (category)
            {
                case TransactionCategories.Food:
                    user.FoodBalance = newBalance;
                    break;
                case TransactionCategories.Meal:
                    user.MealBalance = newBalance;
                    break;
                default:
                    user.CashBalance = newBalance;
                    break;
            }
        }
    }
}

