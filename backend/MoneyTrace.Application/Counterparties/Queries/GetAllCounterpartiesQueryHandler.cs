using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Counterparties.Queries;

internal sealed class GetAllCounterpartiesQueryHandler : IRequestHandler<GetAllCounterpartiesQuery, Result<List<Counterparty>>>
{
    private readonly IRepository<Counterparty> _repository;

    public GetAllCounterpartiesQueryHandler(IRepository<Counterparty> repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<Counterparty>>> Handle(GetAllCounterpartiesQuery request, CancellationToken cancellationToken)
    {
        var counterparties = await _repository.GetAllAsync(cancellationToken);
        return Result.Success(counterparties);
    }
}
