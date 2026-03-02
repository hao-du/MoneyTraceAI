using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Banks.Commands;

internal sealed class CreateBankCommandHandler : IRequestHandler<CreateBankCommand, Result<Guid>>
{
    private readonly IRepository<Bank> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBankCommandHandler(IRepository<Bank> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateBankCommand request, CancellationToken cancellationToken)
    {
        var result = Bank.Create(request.BankName, request.Description);

        if (result.IsFailure) return Result.Failure<Guid>(result.Error);

        _repository.Add(result.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
