using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MoneyTrace.Application.Currencies.Commands;
using MoneyTrace.Application.Currencies.Queries;

namespace MoneyTrace.Api.Endpoints;

public static class CurrencyEndpoints
{
    public static void MapCurrencyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/currencies").WithTags("Currencies").RequireAuthorization();

        group.MapPost("", async (CreateCurrencyCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
        });

        group.MapGet("", async (ISender sender) =>
        {
            var result = await sender.Send(new GetAllCurrenciesQuery());
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
        });

        group.MapPut("{id:guid}", async (Guid id, UpdateCurrencyCommand command, ISender sender) =>
        {
            if (id != command.Id) return Results.BadRequest("ID mismatch");
            
            var result = await sender.Send(command);
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.NoContent();
        });

        group.MapDelete("{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteCurrencyCommand(id));
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.NoContent();
        });
    }
}
