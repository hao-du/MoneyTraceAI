using MediatR;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Users.Commands;

public record CreateUserCommand(string Username, string Password, string FullName, string? Description) : IRequest<Result<Guid>>;
