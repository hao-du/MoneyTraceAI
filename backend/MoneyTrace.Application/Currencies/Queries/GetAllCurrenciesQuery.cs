using MediatR;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Currencies.Queries;

public record GetAllCurrenciesQuery() : IRequest<Result<List<Currency>>>;
