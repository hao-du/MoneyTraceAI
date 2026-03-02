using MediatR;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Counterparties.Queries;

public record GetAllCounterpartiesQuery() : IRequest<Result<List<Counterparty>>>;
