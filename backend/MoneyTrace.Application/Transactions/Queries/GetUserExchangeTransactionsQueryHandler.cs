using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Transactions.Queries;

internal sealed class GetUserExchangeTransactionsQueryHandler : IRequestHandler<GetUserExchangeTransactionsQuery, Result<List<ExchangeTransaction>>>
{
    private readonly IRepository<ExchangeTransaction> _repository;

    public GetUserExchangeTransactionsQueryHandler(IRepository<ExchangeTransaction> repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<ExchangeTransaction>>> Handle(GetUserExchangeTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _repository.GetAllAsync(cancellationToken);
        var userTransactions = transactions.Where(t => t.UserId == request.UserId).OrderByDescending(t => t.DateUtc).ToList();
        
        return Result.Success(userTransactions);
    }
}
