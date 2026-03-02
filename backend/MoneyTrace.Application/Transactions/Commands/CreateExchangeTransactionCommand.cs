using MediatR;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Transactions.Commands;

public record CreateExchangeTransactionCommand(Guid UserId, DateTime DateUtc, decimal SourceAmount, Guid SourceCurrencyId, string? Description, string? Tags,
    decimal TargetAmount, Guid TargetCurrencyId, decimal ExchangeRate) : IRequest<Result<Guid>>;
