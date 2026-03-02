using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Transactions.Queries;

internal sealed class GetUserCashflowsQueryHandler : IRequestHandler<GetUserCashflowsQuery, Result<List<Cashflow>>>
{
    private readonly IRepository<Cashflow> _repository;

    public GetUserCashflowsQueryHandler(IRepository<Cashflow> repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<Cashflow>>> Handle(GetUserCashflowsQuery request, CancellationToken cancellationToken)
    {
        var cashflows = await _repository.GetAllAsync(cancellationToken);
        var userCashflows = cashflows.Where(c => c.UserId == request.UserId).OrderByDescending(c => c.DateUtc).ToList();
        
        return Result.Success(userCashflows);
    }
}
