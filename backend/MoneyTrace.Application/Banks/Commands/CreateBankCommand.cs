using MediatR;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Banks.Commands;

public record CreateBankCommand(string BankName, string? Description) : IRequest<Result<Guid>>;
