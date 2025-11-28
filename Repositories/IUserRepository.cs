using TFinanceWeb.Api.Models;

namespace TFinanceWeb.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByEmail(string email);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<User> DeleteAsync(User user);
    Task<bool> UserExistsByEmail(string email);
    
}