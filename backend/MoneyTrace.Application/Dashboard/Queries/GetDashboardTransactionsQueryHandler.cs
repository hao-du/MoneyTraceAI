using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Application.Dashboard.DTOs;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Dashboard.Queries;

internal sealed class GetDashboardTransactionsQueryHandler : IRequestHandler<GetDashboardTransactionsQuery, Result<List<TransactionDto>>>
{
    private readonly IRepository<Cashflow> _cashflowRepository;
    private readonly IRepository<BankTransaction> _bankRepository;
    private readonly IRepository<ExchangeTransaction> _exchangeRepository;
    private readonly IRepository<TransferTransaction> _transferRepository;

    public GetDashboardTransactionsQueryHandler(
        IRepository<Cashflow> cashflowRepository,
        IRepository<BankTransaction> bankRepository,
        IRepository<ExchangeTransaction> exchangeRepository,
        IRepository<TransferTransaction> transferRepository)
    {
        _cashflowRepository = cashflowRepository;
        _bankRepository = bankRepository;
        _exchangeRepository = exchangeRepository;
        _transferRepository = transferRepository;
    }

    public async Task<Result<List<TransactionDto>>> Handle(GetDashboardTransactionsQuery request, CancellationToken cancellationToken)
    {
        var cashflows = await _cashflowRepository.GetAllAsync(cancellationToken);
        var banks = await _bankRepository.GetAllAsync(cancellationToken);
        var exchanges = await _exchangeRepository.GetAllAsync(cancellationToken);
        var transfers = await _transferRepository.GetAllAsync(cancellationToken);

        var userCashflows = cashflows.Where(t => t.UserId == request.UserId);
        var userBanks = banks.Where(t => t.UserId == request.UserId);
        var userExchanges = exchanges.Where(t => t.UserId == request.UserId);
        var userTransfers = transfers.Where(t => t.UserId == request.UserId);

        if (request.StartDateUtc.HasValue)
        {
            userCashflows = userCashflows.Where(t => t.DateUtc >= request.StartDateUtc.Value);
            userBanks = userBanks.Where(t => t.DateUtc >= request.StartDateUtc.Value);
            userExchanges = userExchanges.Where(t => t.DateUtc >= request.StartDateUtc.Value);
            userTransfers = userTransfers.Where(t => t.DateUtc >= request.StartDateUtc.Value);
        }

        if (request.EndDateUtc.HasValue)
        {
            userCashflows = userCashflows.Where(t => t.DateUtc <= request.EndDateUtc.Value);
            userBanks = userBanks.Where(t => t.DateUtc <= request.EndDateUtc.Value);
            userExchanges = userExchanges.Where(t => t.DateUtc <= request.EndDateUtc.Value);
            userTransfers = userTransfers.Where(t => t.DateUtc <= request.EndDateUtc.Value);
        }

        var results = new List<TransactionDto>();

        results.AddRange(userCashflows.Select(c => new TransactionDto(c.Id, c.DateUtc, c.Amount, c.CurrencyId, c.Description, c.Tags, "Cashflow", c.IsIncome)));
        results.AddRange(userBanks.Select(b => new TransactionDto(b.Id, b.DateUtc, b.Amount, b.CurrencyId, b.Description, b.Tags, "Bank", false))); // Bank deposits/withdraws logic will handle sign
        results.AddRange(userExchanges.Select(e => new TransactionDto(e.Id, e.DateUtc, e.Amount, e.CurrencyId, e.Description, e.Tags, "Exchange", false)));
        results.AddRange(userTransfers.Select(t => new TransactionDto(t.Id, t.DateUtc, t.Amount, t.CurrencyId, t.Description, t.Tags, "Transfer", false)));

        return Result.Success(results.OrderByDescending(r => r.DateUtc).ToList());
    }
}
