using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Domain.Entities;

public class Setting : Entity
{
    private Setting() { } // EF Core
    private Setting(Guid id, Guid userId, string locale, string language, string? metadataJson) : base(id)
    {
        UserId = userId;
        Locale = locale;
        Language = language;
        MetadataJson = metadataJson;
    }

    public Guid UserId { get; private set; }
    public string Locale { get; private set; } = "en-US";
    public string Language { get; private set; } = "en";
    public string? MetadataJson { get; private set; }

    public static Result<Setting> Create(Guid userId, string locale, string language, string? metadataJson)
    {
        if (userId == Guid.Empty)
            return Result.Failure<Setting>(new Error("Setting.UserIdRequired", "User ID is required."));

        return new Setting(Guid.CreateVersion7(), userId, locale, language, metadataJson);
    }

    public void Update(string locale, string language, string? metadataJson)
    {
        Locale = locale;
        Language = language;
        MetadataJson = metadataJson;
    }
}
