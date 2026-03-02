using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Transactions.Commands;

internal sealed class CreateBankTransactionCommandHandler : IRequestHandler<CreateBankTransactionCommand, Result<Guid>>
{
    private readonly IRepository<BankTransaction> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBankTransactionCommandHandler(IRepository<BankTransaction> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateBankTransactionCommand request, CancellationToken cancellationToken)
    {
        var result = BankTransaction.Create(request.UserId, request.DateUtc, request.Amount, request.CurrencyId, request.Description, request.Tags,
            request.AccountNumber, request.BankId, request.InterestPercentage, request.InterestPeriod, 
            request.InterestAmount, request.ActualInterestAmount, request.WithdrawDateUtc);

        if (result.IsFailure) return Result.Failure<Guid>(result.Error);

        _repository.Add(result.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
