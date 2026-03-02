using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Currencies.Commands;

internal sealed class CreateCurrencyCommandHandler : IRequestHandler<CreateCurrencyCommand, Result<Guid>>
{
    private readonly IRepository<Currency> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCurrencyCommandHandler(IRepository<Currency> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var result = Currency.Create(request.CurrencyName, request.CurrencyShortName, request.Description);

        if (result.IsFailure) return Result.Failure<Guid>(result.Error);

        _repository.Add(result.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
