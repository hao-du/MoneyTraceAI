using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Transactions.Queries;

internal sealed class GetUserTransferTransactionsQueryHandler : IRequestHandler<GetUserTransferTransactionsQuery, Result<List<TransferTransaction>>>
{
    private readonly IRepository<TransferTransaction> _repository;

    public GetUserTransferTransactionsQueryHandler(IRepository<TransferTransaction> repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<TransferTransaction>>> Handle(GetUserTransferTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _repository.GetAllAsync(cancellationToken);
        var userTransactions = transactions.Where(t => t.UserId == request.UserId).OrderByDescending(t => t.DateUtc).ToList();
        
        return Result.Success(userTransactions);
    }
}
