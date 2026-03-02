using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Banks.Queries;

internal sealed class GetAllBanksQueryHandler : IRequestHandler<GetAllBanksQuery, Result<List<Bank>>>
{
    private readonly IRepository<Bank> _repository;

    public GetAllBanksQueryHandler(IRepository<Bank> repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<Bank>>> Handle(GetAllBanksQuery request, CancellationToken cancellationToken)
    {
        var banks = await _repository.GetAllAsync(cancellationToken);
        return Result.Success(banks);
    }
}
