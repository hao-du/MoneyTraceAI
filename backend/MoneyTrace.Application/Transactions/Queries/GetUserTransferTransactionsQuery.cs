using MediatR;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Transactions.Queries;

public record GetUserTransferTransactionsQuery(Guid UserId) : IRequest<Result<List<TransferTransaction>>>;
