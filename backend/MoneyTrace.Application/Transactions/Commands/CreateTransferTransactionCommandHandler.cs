using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Transactions.Commands;

internal sealed class CreateTransferTransactionCommandHandler : IRequestHandler<CreateTransferTransactionCommand, Result<Guid>>
{
    private readonly IRepository<TransferTransaction> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTransferTransactionCommandHandler(IRepository<TransferTransaction> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateTransferTransactionCommand request, CancellationToken cancellationToken)
    {
        var result = TransferTransaction.Create(request.UserId, request.DateUtc, request.Amount, request.CurrencyId, request.Description, request.Tags,
            request.CounterpartyId, request.TransferType, request.Status);

        if (result.IsFailure) return Result.Failure<Guid>(result.Error);

        _repository.Add(result.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
