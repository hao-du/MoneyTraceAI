using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Banks.Commands;

internal sealed class UpdateBankCommandHandler : IRequestHandler<UpdateBankCommand, Result>
{
    private readonly IRepository<Bank> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBankCommandHandler(IRepository<Bank> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateBankCommand request, CancellationToken cancellationToken)
    {
        var bank = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (bank is null) return Result.Failure(new Error("Bank.NotFound", "The bank was not found."));

        bank.Update(request.BankName, request.Description);
        
        _repository.Update(bank);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
