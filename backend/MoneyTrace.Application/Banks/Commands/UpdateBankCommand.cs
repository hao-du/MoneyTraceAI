using MediatR;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Banks.Commands;

public record UpdateBankCommand(Guid Id, string BankName, string? Description) : IRequest<Result>;
