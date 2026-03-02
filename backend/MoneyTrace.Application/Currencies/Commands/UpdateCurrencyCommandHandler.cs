using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Currencies.Commands;

internal sealed class UpdateCurrencyCommandHandler : IRequestHandler<UpdateCurrencyCommand, Result>
{
    private readonly IRepository<Currency> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCurrencyCommandHandler(IRepository<Currency> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var currency = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (currency is null) return Result.Failure(new Error("Currency.NotFound", "The currency was not found."));

        currency.Update(request.CurrencyName, request.CurrencyShortName, request.Description);
        
        _repository.Update(currency);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
