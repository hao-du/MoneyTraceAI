namespace MoneyTrace.Application.Dashboard.DTOs;

public record TransactionDto(
    Guid Id, 
    DateTime DateUtc, 
    decimal Amount, 
    Guid CurrencyId, 
    string? Description, 
    string? Tags, 
    string Type, 
    bool IsIncome
);
