using MoneyTrace.Domain.Enums;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Domain.Entities;

public class TransferTransaction : Transaction
{
    private TransferTransaction() { } // EF Core

    private TransferTransaction(Guid id, Guid userId, DateTime dateUtc, decimal amount, Guid currencyId, string? description, string? tags,
        Guid counterpartyId, TransferType transferType, TransferStatus status)
        : base(id, userId, dateUtc, amount, currencyId, description, tags, TransactionType.Transfer)
    {
        CounterpartyId = counterpartyId;
        TransferType = transferType;
        Status = status;
    }

    public Guid CounterpartyId { get; private set; }
    public TransferType TransferType { get; private set; }
    public TransferStatus Status { get; private set; }

    // Navigation
    private readonly List<TransferTransactionDetail> _details = new();
    public IReadOnlyCollection<TransferTransactionDetail> Details => _details.AsReadOnly();

    public static Result<TransferTransaction> Create(Guid userId, DateTime dateUtc, decimal amount, Guid currencyId, string? description, string? tags,
        Guid counterpartyId, TransferType transferType, TransferStatus status)
    {
        return Result.Success(new TransferTransaction(Guid.CreateVersion7(), userId, dateUtc, amount, currencyId, description, tags, counterpartyId, transferType, status));
    }

    public void AddDetail(DateTime dateUtc, decimal amount, string? note)
    {
        _details.Add(TransferTransactionDetail.Create(Id, dateUtc, amount, note));
    }

    public void UpdateStatus(TransferStatus status)
    {
        Status = status;
    }
}

public class TransferTransactionDetail : Primitives.Entity
{
    private TransferTransactionDetail() { }

    private TransferTransactionDetail(Guid id, Guid transferTransactionId, DateTime dateUtc, decimal amount, string? note) : base(id)
    {
        TransferTransactionId = transferTransactionId;
        DateUtc = dateUtc;
        Amount = amount;
        Note = note;
    }

    public Guid TransferTransactionId { get; private set; }
    public DateTime DateUtc { get; private set; }
    public decimal Amount { get; private set; }
    public string? Note { get; private set; }

    internal static TransferTransactionDetail Create(Guid transferTransactionId, DateTime dateUtc, decimal amount, string? note)
    {
        return new TransferTransactionDetail(Guid.CreateVersion7(), transferTransactionId, dateUtc, amount, note);
    }
}
