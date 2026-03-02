using MediatR;
using MoneyTrace.Domain.Primitives;
using MoneyTrace.Application.Dashboard.DTOs;

namespace MoneyTrace.Application.Dashboard.Queries;

public record GetDashboardTransactionsQuery(Guid UserId, DateTime? StartDateUtc, DateTime? EndDateUtc) : IRequest<Result<List<TransactionDto>>>;
