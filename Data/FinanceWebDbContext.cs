using Microsoft.EntityFrameworkCore;
using TFinanceWeb.Api.Models;

namespace TFinanceWeb.Api.Data;

public class FinanceWebDbContext : DbContext
{
    public FinanceWebDbContext(DbContextOptions<FinanceWebDbContext> options) : base(options)
    {
    }
    public DbSet<User> Users => Set<User>();
    // public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    // public DbSet<Transaction> Transactions => Set<Transaction>();
    // public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceWebDbContext).Assembly);
    }
}