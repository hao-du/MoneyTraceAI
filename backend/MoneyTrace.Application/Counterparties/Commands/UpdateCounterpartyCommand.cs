using MediatR;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Counterparties.Commands;

public record UpdateCounterpartyCommand(Guid Id, string FullName, string? ContactNumber, string? EmailAddress, string? HomeAddress, string? Description) : IRequest<Result>;
