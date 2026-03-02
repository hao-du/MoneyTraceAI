using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MoneyTrace.Application.Dashboard.Queries;

namespace MoneyTrace.Api.Endpoints;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/dashboard").WithTags("Dashboard").RequireAuthorization();

        group.MapGet("transactions", async (DateTime? startDateUtc, DateTime? endDateUtc, ClaimsPrincipal user, ISender sender) =>
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId)) return Results.Unauthorized();

            var result = await sender.Send(new GetDashboardTransactionsQuery(userId, startDateUtc, endDateUtc));
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
        });
    }
}
