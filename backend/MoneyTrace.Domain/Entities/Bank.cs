using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Domain.Entities;

public class Bank : Entity
{
    private Bank() { } // EF Core

    private Bank(Guid id, string bankName, string? description) : base(id)
    {
        BankName = bankName;
        Description = description;
    }

    public string BankName { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public static Result<Bank> Create(string bankName, string? description)
    {
        if (string.IsNullOrWhiteSpace(bankName))
            return Result.Failure<Bank>(new Error("Bank.BankNameRequired", "Bank Name is required."));

        return new Bank(Guid.CreateVersion7(), bankName, description);
    }

    public void Update(string bankName, string? description)
    {
        BankName = bankName;
        Description = description;
    }
}
