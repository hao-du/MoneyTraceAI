using MediatR;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Settings.Queries;

public record GetSettingQuery(Guid UserId) : IRequest<Result<Setting>>;
