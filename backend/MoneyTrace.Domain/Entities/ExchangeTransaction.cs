using MoneyTrace.Domain.Enums;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Domain.Entities;

public class ExchangeTransaction : Transaction
{
    private ExchangeTransaction() { } // EF Core

    private ExchangeTransaction(Guid id, Guid userId, DateTime dateUtc, decimal amount, Guid currencyId, string? description, string? tags,
        decimal targetAmount, Guid targetCurrencyId, decimal exchangeRate)
        : base(id, userId, dateUtc, amount, currencyId, description, tags, TransactionType.Exchange)
    {
        TargetAmount = targetAmount;
        TargetCurrencyId = targetCurrencyId;
        ExchangeRate = exchangeRate;
    }

    // Amount and CurrencyId from base play the role of Source Amount and Source Currency
    public decimal TargetAmount { get; private set; }
    public Guid TargetCurrencyId { get; private set; }
    public decimal ExchangeRate { get; private set; }

    public static Result<ExchangeTransaction> Create(Guid userId, DateTime dateUtc, decimal sourceAmount, Guid sourceCurrencyId, string? description, string? tags,
        decimal targetAmount, Guid targetCurrencyId, decimal exchangeRate)
    {
        return Result.Success(new ExchangeTransaction(Guid.CreateVersion7(), userId, dateUtc, sourceAmount, sourceCurrencyId, description, tags, targetAmount, targetCurrencyId, exchangeRate));
    }
}
