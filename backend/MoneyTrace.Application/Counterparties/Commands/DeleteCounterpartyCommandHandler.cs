using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Counterparties.Commands;

internal sealed class DeleteCounterpartyCommandHandler : IRequestHandler<DeleteCounterpartyCommand, Result>
{
    private readonly IRepository<Counterparty> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCounterpartyCommandHandler(IRepository<Counterparty> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCounterpartyCommand request, CancellationToken cancellationToken)
    {
        var counterparty = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (counterparty is null) return Result.Failure(new Error("Counterparty.NotFound", "The counterparty was not found."));

        _repository.Remove(counterparty);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
