using MediatR;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Settings.Commands;

public record SaveSettingCommand(Guid UserId, string Locale, string Language, string? MetadataJson) : IRequest<Result>;
