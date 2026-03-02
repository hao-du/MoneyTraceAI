using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Transactions.Commands;

internal sealed class CreateExchangeTransactionCommandHandler : IRequestHandler<CreateExchangeTransactionCommand, Result<Guid>>
{
    private readonly IRepository<ExchangeTransaction> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateExchangeTransactionCommandHandler(IRepository<ExchangeTransaction> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateExchangeTransactionCommand request, CancellationToken cancellationToken)
    {
        var result = ExchangeTransaction.Create(request.UserId, request.DateUtc, request.SourceAmount, request.SourceCurrencyId, request.Description, request.Tags,
            request.TargetAmount, request.TargetCurrencyId, request.ExchangeRate);

        if (result.IsFailure) return Result.Failure<Guid>(result.Error);

        _repository.Add(result.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
