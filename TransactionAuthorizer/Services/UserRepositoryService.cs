using TransactionAuthorizer.Models;

namespace TransactionAuthorizer.Services
{
    public class UserRepositoryService : IUserRepositoryService
    {
        private static readonly Dictionary<string, UserAccount> _users = new()
    {
        { "123", new UserAccount { Account = "123", FoodBalance = 700, MealBalance = 500, CashBalance = 2000 } },
        { "user_001", new UserAccount { Account = "user_001", FoodBalance = 500, MealBalance = 300, CashBalance = 1000 } },
        { "user_002", new UserAccount { Account = "user_002", FoodBalance = 400, MealBalance = 250, CashBalance = 1200 } },
        { "user_003", new UserAccount { Account = "user_003", FoodBalance = 600, MealBalance = 320, CashBalance = 900 } },
        { "user_004", new UserAccount { Account = "user_004", FoodBalance = 700, MealBalance = 400, CashBalance = 800 } },
        { "user_005", new UserAccount { Account = "user_005", FoodBalance = 550, MealBalance = 350, CashBalance = 1100 } }
    };

        public UserAccount? GetUser(string account) => _users.ContainsKey(account) ? _users[account] : null;

        public IEnumerable<UserAccount> GetAllUsers() => _users.Values;
    }
}
