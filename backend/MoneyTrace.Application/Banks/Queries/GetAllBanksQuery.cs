using MediatR;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Banks.Queries;

public record GetAllBanksQuery() : IRequest<Result<List<Bank>>>;
