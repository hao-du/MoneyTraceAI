using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Banks.Commands;

internal sealed class DeleteBankCommandHandler : IRequestHandler<DeleteBankCommand, Result>
{
    private readonly IRepository<Bank> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBankCommandHandler(IRepository<Bank> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteBankCommand request, CancellationToken cancellationToken)
    {
        var bank = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (bank is null) return Result.Failure(new Error("Bank.NotFound", "The bank was not found."));

        _repository.Remove(bank); // soft deletes under the hood
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
