using MoneyTrace.Domain.Enums;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Domain.Entities;

public class BankTransaction : Transaction
{
    private BankTransaction() { } // EF Core

    private BankTransaction(Guid id, Guid userId, DateTime dateUtc, decimal amount, Guid currencyId, string? description, string? tags,
        string accountNumber, Guid bankId, decimal interestPercentage, InterestPeriod interestPeriod, 
        decimal interestAmount, decimal actualInterestAmount, DateTime? withdrawDateUtc, bool isClose)
        : base(id, userId, dateUtc, amount, currencyId, description, tags, TransactionType.Bank)
    {
        AccountNumber = accountNumber;
        BankId = bankId;
        InterestPercentage = interestPercentage;
        InterestPeriod = interestPeriod;
        InterestAmount = interestAmount;
        ActualInterestAmount = actualInterestAmount;
        WithdrawDateUtc = withdrawDateUtc;
        IsClose = isClose;
    }

    public string AccountNumber { get; private set; } = string.Empty;
    public Guid BankId { get; private set; }
    public decimal InterestPercentage { get; private set; }
    public InterestPeriod InterestPeriod { get; private set; }
    public decimal InterestAmount { get; private set; } // Auto calculated
    public decimal ActualInterestAmount { get; private set; } // Manual input
    public DateTime? WithdrawDateUtc { get; private set; }
    public bool IsClose { get; private set; }

    // Navigation
    private readonly List<BankTransactionDetail> _details = new();
    public IReadOnlyCollection<BankTransactionDetail> Details => _details.AsReadOnly();

    public static Result<BankTransaction> Create(Guid userId, DateTime dateUtc, decimal amount, Guid currencyId, string? description, string? tags,
        string accountNumber, Guid bankId, decimal interestPercentage, InterestPeriod interestPeriod, 
        decimal interestAmount, decimal actualInterestAmount, DateTime? withdrawDateUtc)
    {
        return Result.Success(new BankTransaction(Guid.CreateVersion7(), userId, dateUtc, amount, currencyId, description, tags,
            accountNumber, bankId, interestPercentage, interestPeriod, interestAmount, actualInterestAmount, withdrawDateUtc, false));
    }

    public void CloseAccount(DateTime withdrawDateUtc, decimal actualInterestAmount)
    {
        if (withdrawDateUtc > DateTime.UtcNow)
            throw new InvalidOperationException("Cannot close account with future withdraw date");

        IsClose = true;
        WithdrawDateUtc = withdrawDateUtc;
        ActualInterestAmount = actualInterestAmount;
    }

    public void AddDetail(DateTime dateUtc, decimal interest)
    {
        _details.Add(BankTransactionDetail.Create(Id, dateUtc, interest));
    }
}

public class BankTransactionDetail : Primitives.Entity
{
    private BankTransactionDetail() { }

    private BankTransactionDetail(Guid id, Guid bankTransactionId, DateTime dateUtc, decimal interest) : base(id)
    {
        BankTransactionId = bankTransactionId;
        DateUtc = dateUtc;
        Interest = interest;
    }

    public Guid BankTransactionId { get; private set; }
    public DateTime DateUtc { get; private set; }
    public decimal Interest { get; private set; }

    internal static BankTransactionDetail Create(Guid bankTransactionId, DateTime dateUtc, decimal interest)
    {
        return new BankTransactionDetail(Guid.CreateVersion7(), bankTransactionId, dateUtc, interest);
    }
}
