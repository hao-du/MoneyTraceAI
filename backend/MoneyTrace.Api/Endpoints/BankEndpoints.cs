using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MoneyTrace.Application.Banks.Commands;
using MoneyTrace.Application.Banks.Queries;

namespace MoneyTrace.Api.Endpoints;

public static class BankEndpoints
{
    public static void MapBankEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/banks").WithTags("Banks").RequireAuthorization();

        group.MapPost("", async (CreateBankCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
        });

        group.MapGet("", async (ISender sender) =>
        {
            var result = await sender.Send(new GetAllBanksQuery());
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
        });

        group.MapPut("{id:guid}", async (Guid id, UpdateBankCommand command, ISender sender) =>
        {
            if (id != command.Id) return Results.BadRequest("ID mismatch");
            
            var result = await sender.Send(command);
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.NoContent();
        });

        group.MapDelete("{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteBankCommand(id));
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.NoContent();
        });
    }
}
