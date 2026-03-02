using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MoneyTrace.Application.Counterparties.Commands;
using MoneyTrace.Application.Counterparties.Queries;

namespace MoneyTrace.Api.Endpoints;

public static class CounterpartyEndpoints
{
    public static void MapCounterpartyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/counterparties").WithTags("Counterparties").RequireAuthorization();

        group.MapPost("", async (CreateCounterpartyCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
        });

        group.MapGet("", async (ISender sender) =>
        {
            var result = await sender.Send(new GetAllCounterpartiesQuery());
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
        });

        group.MapPut("{id:guid}", async (Guid id, UpdateCounterpartyCommand command, ISender sender) =>
        {
            if (id != command.Id) return Results.BadRequest("ID mismatch");
            
            var result = await sender.Send(command);
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.NoContent();
        });

        group.MapDelete("{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteCounterpartyCommand(id));
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.NoContent();
        });
    }
}
