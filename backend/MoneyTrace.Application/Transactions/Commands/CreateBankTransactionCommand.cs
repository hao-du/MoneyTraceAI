using MediatR;
using MoneyTrace.Domain.Enums;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Transactions.Commands;

public record CreateBankTransactionCommand(Guid UserId, DateTime DateUtc, decimal Amount, Guid CurrencyId, string? Description, string? Tags,
    string AccountNumber, Guid BankId, decimal InterestPercentage, InterestPeriod InterestPeriod, 
    decimal InterestAmount, decimal ActualInterestAmount, DateTime? WithdrawDateUtc) : IRequest<Result<Guid>>;
