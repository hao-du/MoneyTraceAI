using Microsoft.EntityFrameworkCore;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;

namespace MoneyTrace.Infrastructure.Data;

public class MoneyTraceDbContext : DbContext, IUnitOfWork
{
    public MoneyTraceDbContext(DbContextOptions<MoneyTraceDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Counterparty> Counterparties => Set<Counterparty>();
    public DbSet<Bank> Banks => Set<Bank>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    public DbSet<Cashflow> Cashflows => Set<Cashflow>();
    public DbSet<BankTransaction> BankTransactions => Set<BankTransaction>();
    public DbSet<ExchangeTransaction> ExchangeTransactions => Set<ExchangeTransaction>();
    public DbSet<TransferTransaction> TransferTransactions => Set<TransferTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MoneyTraceDbContext).Assembly);
    }
}
