using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MoneyTrace.Application.Settings.Commands;
using MoneyTrace.Application.Settings.Queries;

namespace MoneyTrace.Api.Endpoints;

public static class SettingsEndpoints
{
    public static void MapSettingsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/settings").WithTags("Settings").RequireAuthorization();

        group.MapGet("", async (ClaimsPrincipal user, ISender sender) =>
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId)) return Results.Unauthorized();

            var result = await sender.Send(new GetSettingQuery(userId));
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
        });

        group.MapPost("", async (SaveSettingCommand command, ClaimsPrincipal user, ISender sender) =>
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId) || command.UserId != userId) 
                return Results.Unauthorized();

            var result = await sender.Send(command);
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.NoContent();
        });
    }
}
