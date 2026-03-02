using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Domain.Entities;

public class Currency : Entity
{
    private Currency() { } // EF Core
    private Currency(Guid id, string currencyName, string currencyShortName, string? description) : base(id)
    {
        CurrencyName = currencyName;
        CurrencyShortName = currencyShortName;
        Description = description;
    }

    public string CurrencyName { get; private set; } = string.Empty;
    public string CurrencyShortName { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public static Result<Currency> Create(string currencyName, string currencyShortName, string? description)
    {
        if (string.IsNullOrWhiteSpace(currencyName))
            return Result.Failure<Currency>(new Error("Currency.NameRequired", "Currency Name is required."));

        if (string.IsNullOrWhiteSpace(currencyShortName))
            return Result.Failure<Currency>(new Error("Currency.ShortNameRequired", "Currency Short Name is required."));

        return new Currency(Guid.CreateVersion7(), currencyName, currencyShortName, description);
    }

    public void Update(string currencyName, string currencyShortName, string? description)
    {
        CurrencyName = currencyName;
        CurrencyShortName = currencyShortName;
        Description = description;
    }
}
