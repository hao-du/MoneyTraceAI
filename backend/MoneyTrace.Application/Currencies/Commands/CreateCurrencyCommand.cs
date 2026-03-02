using MediatR;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Currencies.Commands;

public record CreateCurrencyCommand(string CurrencyName, string CurrencyShortName, string? Description) : IRequest<Result<Guid>>;
