using MediatR;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Counterparties.Commands;

public record DeleteCounterpartyCommand(Guid Id) : IRequest<Result>;
