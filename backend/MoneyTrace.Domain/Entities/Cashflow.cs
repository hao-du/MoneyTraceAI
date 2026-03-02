using MoneyTrace.Domain.Enums;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Domain.Entities;

public class Cashflow : Transaction
{
    private Cashflow() { } // EF Core

    private Cashflow(Guid id, Guid userId, DateTime dateUtc, decimal amount, Guid currencyId, string? description, string? tags, bool isIncome)
        : base(id, userId, dateUtc, amount, currencyId, description, tags, TransactionType.Cashflow)
    {
        IsIncome = isIncome;
    }

    public bool IsIncome { get; private set; }
    
    // Navigation
    private readonly List<CashflowItem> _items = new();
    public IReadOnlyCollection<CashflowItem> Items => _items.AsReadOnly();

    public static Result<Cashflow> Create(Guid userId, DateTime dateUtc, decimal amount, Guid currencyId, string? description, string? tags, bool isIncome)
    {
        return Result.Success(new Cashflow(Guid.CreateVersion7(), userId, dateUtc, amount, currencyId, description, tags, isIncome));
    }

    public void AddItem(string itemName, decimal amount, string? description)
    {
        _items.Add(CashflowItem.Create(Id, itemName, amount, description));
    }
}

public class CashflowItem : Primitives.Entity
{
    private CashflowItem() { }

    private CashflowItem(Guid id, Guid cashflowId, string itemName, decimal amount, string? description) : base(id)
    {
        CashflowId = cashflowId;
        ItemName = itemName;
        Amount = amount;
        Description = description;
    }

    public Guid CashflowId { get; private set; }
    public string ItemName { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string? Description { get; private set; }

    internal static CashflowItem Create(Guid cashflowId, string itemName, decimal amount, string? description)
    {
        return new CashflowItem(Guid.CreateVersion7(), cashflowId, itemName, amount, description);
    }
}
