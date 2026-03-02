using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Settings.Queries;

internal sealed class GetSettingQueryHandler : IRequestHandler<GetSettingQuery, Result<Setting>>
{
    private readonly IRepository<Setting> _repository;

    public GetSettingQueryHandler(IRepository<Setting> repository)
    {
        _repository = repository;
    }

    public async Task<Result<Setting>> Handle(GetSettingQuery request, CancellationToken cancellationToken)
    {
        var settings = await _repository.GetAllAsync(cancellationToken);
        var userSetting = settings.FirstOrDefault(s => s.UserId == request.UserId);

        if (userSetting is null)
        {
            // Return a default setting if none exists yet, but without saving it.
            var defaultSettingResult = Setting.Create(request.UserId, "en-US", "en", null);
            return Result.Success(defaultSettingResult.Value);
        }

        return Result.Success(userSetting);
    }
}
