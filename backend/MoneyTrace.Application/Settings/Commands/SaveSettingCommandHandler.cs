using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Settings.Commands;

internal sealed class SaveSettingCommandHandler : IRequestHandler<SaveSettingCommand, Result>
{
    private readonly IRepository<Setting> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SaveSettingCommandHandler(IRepository<Setting> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SaveSettingCommand request, CancellationToken cancellationToken)
    {
        var settings = await _repository.GetAllAsync(cancellationToken);
        var existingSetting = settings.FirstOrDefault(s => s.UserId == request.UserId);

        if (existingSetting is not null)
        {
            existingSetting.Update(request.Locale, request.Language, request.MetadataJson);
            _repository.Update(existingSetting);
        }
        else
        {
            var newSettingResult = Setting.Create(request.UserId, request.Locale, request.Language, request.MetadataJson);
            if (newSettingResult.IsFailure) return Result.Failure(newSettingResult.Error);
            _repository.Add(newSettingResult.Value);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
