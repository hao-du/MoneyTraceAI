using MediatR;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Transactions.Queries;

public record GetUserBankTransactionsQuery(Guid UserId) : IRequest<Result<List<BankTransaction>>>;
