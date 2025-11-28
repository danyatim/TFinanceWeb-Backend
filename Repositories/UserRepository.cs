using Microsoft.EntityFrameworkCore;
using TFinanceWeb.Api.Data;
using TFinanceWeb.Api.Models;

namespace TFinanceWeb.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly FinanceWebDbContext _context;
    
    public UserRepository(FinanceWebDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
    }
    
    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
    
    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }
    
    public async Task<User> DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return user;
    }
    
    public async Task<bool> UserExistsByEmail(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == email);
    }
}