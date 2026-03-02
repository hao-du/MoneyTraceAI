using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Transactions.Queries;

internal sealed class GetUserBankTransactionsQueryHandler : IRequestHandler<GetUserBankTransactionsQuery, Result<List<BankTransaction>>>
{
    private readonly IRepository<BankTransaction> _repository;

    public GetUserBankTransactionsQueryHandler(IRepository<BankTransaction> repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<BankTransaction>>> Handle(GetUserBankTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _repository.GetAllAsync(cancellationToken);
        var userTransactions = transactions.Where(t => t.UserId == request.UserId).OrderByDescending(t => t.DateUtc).ToList();
        
        return Result.Success(userTransactions);
    }
}
