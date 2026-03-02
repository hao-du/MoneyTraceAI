using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Counterparties.Commands;

internal sealed class CreateCounterpartyCommandHandler : IRequestHandler<CreateCounterpartyCommand, Result<Guid>>
{
    private readonly IRepository<Counterparty> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCounterpartyCommandHandler(IRepository<Counterparty> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCounterpartyCommand request, CancellationToken cancellationToken)
    {
        var result = Counterparty.Create(request.FullName, request.ContactNumber, request.EmailAddress, request.HomeAddress, request.Description);

        if (result.IsFailure) return Result.Failure<Guid>(result.Error);

        _repository.Add(result.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
