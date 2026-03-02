using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Transactions.Commands;

internal sealed class CreateCashflowCommandHandler : IRequestHandler<CreateCashflowCommand, Result<Guid>>
{
    private readonly IRepository<Cashflow> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCashflowCommandHandler(IRepository<Cashflow> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCashflowCommand request, CancellationToken cancellationToken)
    {
        var result = Cashflow.Create(request.UserId, request.DateUtc, request.Amount, request.CurrencyId, request.Description, request.Tags, request.IsIncome);

        if (result.IsFailure) return Result.Failure<Guid>(result.Error);

        _repository.Add(result.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
