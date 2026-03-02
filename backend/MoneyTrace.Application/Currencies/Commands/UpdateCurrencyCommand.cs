using MediatR;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Currencies.Commands;

public record UpdateCurrencyCommand(Guid Id, string CurrencyName, string CurrencyShortName, string? Description) : IRequest<Result>;
