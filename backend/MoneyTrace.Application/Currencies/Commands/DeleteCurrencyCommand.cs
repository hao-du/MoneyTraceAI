using MediatR;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Currencies.Commands;

public record DeleteCurrencyCommand(Guid Id) : IRequest<Result>;
