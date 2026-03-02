using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Counterparties.Commands;

internal sealed class UpdateCounterpartyCommandHandler : IRequestHandler<UpdateCounterpartyCommand, Result>
{
    private readonly IRepository<Counterparty> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCounterpartyCommandHandler(IRepository<Counterparty> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCounterpartyCommand request, CancellationToken cancellationToken)
    {
        var counterparty = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (counterparty is null) return Result.Failure(new Error("Counterparty.NotFound", "The counterparty was not found."));

        counterparty.Update(request.FullName, request.ContactNumber, request.EmailAddress, request.HomeAddress, request.Description);
        
        _repository.Update(counterparty);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
