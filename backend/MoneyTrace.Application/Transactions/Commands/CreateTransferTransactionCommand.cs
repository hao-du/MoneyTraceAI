using MediatR;
using MoneyTrace.Domain.Enums;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Transactions.Commands;

public record CreateTransferTransactionCommand(Guid UserId, DateTime DateUtc, decimal Amount, Guid CurrencyId, string? Description, string? Tags,
    Guid CounterpartyId, TransferType TransferType, TransferStatus Status) : IRequest<Result<Guid>>;
