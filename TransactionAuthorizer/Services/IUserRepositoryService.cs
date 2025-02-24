using TransactionAuthorizer.Models;

namespace TransactionAuthorizer.Services
{
    public interface IUserRepositoryService
    {
        UserAccount? GetUser(string account);
        IEnumerable<UserAccount> GetAllUsers();
    }
}
