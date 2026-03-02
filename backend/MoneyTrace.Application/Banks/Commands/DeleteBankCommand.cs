using MediatR;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Banks.Commands;

public record DeleteBankCommand(Guid Id) : IRequest<Result>;
