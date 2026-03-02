using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Domain.Entities;

// Base Transaction Class
public abstract class Transaction : Entity
{
    protected Transaction() { } // EF Core

    protected Transaction(Guid id, Guid userId, DateTime dateUtc, decimal amount, Guid currencyId, string? description, string? tags, Enums.TransactionType type) 
        : base(id)
    {
        UserId = userId;
        DateUtc = dateUtc;
        Amount = amount;
        CurrencyId = currencyId;
        Description = description;
        Tags = tags;
        Type = type;
    }

    public Guid UserId { get; protected set; }
    public DateTime DateUtc { get; protected set; }
    public decimal Amount { get; protected set; }
    public Guid CurrencyId { get; protected set; }
    public string? Description { get; protected set; }
    public string? Tags { get; protected set; } // Comma separated tags e.g. "Cashflow,Food"
    public Enums.TransactionType Type { get; protected set; }
}
