using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MoneyTrace.Application.Transactions.Commands;
using MoneyTrace.Application.Transactions.Queries;

namespace MoneyTrace.Api.Endpoints;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/transactions").WithTags("Transactions").RequireAuthorization();

        // --- Cashflow ---
        group.MapGet("cashflow", async (ClaimsPrincipal user, ISender sender) =>
        {
            if (!TryGetUserId(user, out var userId)) return Results.Unauthorized();
            var result = await sender.Send(new GetUserCashflowsQuery(userId));
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
        });

        group.MapPost("cashflow", async (CreateCashflowCommand command, ClaimsPrincipal user, ISender sender) =>
        {
            if (!TryGetUserId(user, out var userId) || command.UserId != userId) return Results.Unauthorized();
            var result = await sender.Send(command);
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
        });

        // --- Bank ---
        group.MapGet("bank", async (ClaimsPrincipal user, ISender sender) =>
        {
            if (!TryGetUserId(user, out var userId)) return Results.Unauthorized();
            var result = await sender.Send(new GetUserBankTransactionsQuery(userId));
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
        });

        group.MapPost("bank", async (CreateBankTransactionCommand command, ClaimsPrincipal user, ISender sender) =>
        {
            if (!TryGetUserId(user, out var userId) || command.UserId != userId) return Results.Unauthorized();
            var result = await sender.Send(command);
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
        });

        // --- Exchange ---
        group.MapGet("exchange", async (ClaimsPrincipal user, ISender sender) =>
        {
            if (!TryGetUserId(user, out var userId)) return Results.Unauthorized();
            var result = await sender.Send(new GetUserExchangeTransactionsQuery(userId));
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
        });

        group.MapPost("exchange", async (CreateExchangeTransactionCommand command, ClaimsPrincipal user, ISender sender) =>
        {
            if (!TryGetUserId(user, out var userId) || command.UserId != userId) return Results.Unauthorized();
            var result = await sender.Send(command);
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
        });

        // --- Transfer ---
        group.MapGet("transfer", async (ClaimsPrincipal user, ISender sender) =>
        {
            if (!TryGetUserId(user, out var userId)) return Results.Unauthorized();
            var result = await sender.Send(new GetUserTransferTransactionsQuery(userId));
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
        });

        group.MapPost("transfer", async (CreateTransferTransactionCommand command, ClaimsPrincipal user, ISender sender) =>
        {
            if (!TryGetUserId(user, out var userId) || command.UserId != userId) return Results.Unauthorized();
            var result = await sender.Send(command);
            return result.IsFailure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
        });
    }

    private static bool TryGetUserId(ClaimsPrincipal principal, out Guid userId)
    {
        userId = Guid.Empty;
        var userIdStr = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdStr, out userId);
    }
}
