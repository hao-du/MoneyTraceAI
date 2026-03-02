using MediatR;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Transactions.Queries;

public record GetUserCashflowsQuery(Guid UserId) : IRequest<Result<List<Cashflow>>>;
