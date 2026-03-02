using MediatR;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Users.Queries;

public record LoginUserQuery(string Username, string Password) : IRequest<Result<string>>;
