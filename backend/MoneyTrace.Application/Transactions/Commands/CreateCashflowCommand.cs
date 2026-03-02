using MediatR;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Transactions.Commands;

public record CreateCashflowCommand(Guid UserId, DateTime DateUtc, decimal Amount, Guid CurrencyId, string? Description, string? Tags, bool IsIncome) : IRequest<Result<Guid>>;
