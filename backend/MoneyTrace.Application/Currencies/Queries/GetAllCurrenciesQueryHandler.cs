using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Currencies.Queries;

internal sealed class GetAllCurrenciesQueryHandler : IRequestHandler<GetAllCurrenciesQuery, Result<List<Currency>>>
{
    private readonly IRepository<Currency> _repository;

    public GetAllCurrenciesQueryHandler(IRepository<Currency> repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<Currency>>> Handle(GetAllCurrenciesQuery request, CancellationToken cancellationToken)
    {
        var currencies = await _repository.GetAllAsync(cancellationToken);
        return Result.Success(currencies);
    }
}
