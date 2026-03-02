using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MoneyTrace.Application.Users.Commands;
using MoneyTrace.Application.Users.Queries;

namespace MoneyTrace.Api.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/auth").WithTags("Authentication");

        group.MapPost("register", async (CreateUserCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        });

        group.MapPost("login", async (LoginUserQuery query, ISender sender) =>
        {
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(new { Token = result.Value });
        });
    }
}
