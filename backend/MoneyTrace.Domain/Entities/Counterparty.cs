using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Domain.Entities;

public class Counterparty : Entity
{
    private Counterparty() { } // EF Core
    private Counterparty(Guid id, string fullName, string? contactNumber, string? emailAddress, string? homeAddress, string? description)
        : base(id)
    {
        FullName = fullName;
        ContactNumber = contactNumber;
        EmailAddress = emailAddress;
        HomeAddress = homeAddress;
        Description = description;
    }

    public string FullName { get; private set; } = string.Empty;
    public string? ContactNumber { get; private set; }
    public string? EmailAddress { get; private set; }
    public string? HomeAddress { get; private set; }
    public string? Description { get; private set; }

    public static Result<Counterparty> Create(string fullName, string? contactNumber, string? emailAddress, string? homeAddress, string? description)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return Result.Failure<Counterparty>(new Error("Counterparty.FullNameRequired", "Full Name is required."));

        return new Counterparty(Guid.CreateVersion7(), fullName, contactNumber, emailAddress, homeAddress, description);
    }

    public void Update(string fullName, string? contactNumber, string? emailAddress, string? homeAddress, string? description)
    {
        FullName = fullName;
        ContactNumber = contactNumber;
        EmailAddress = emailAddress;
        HomeAddress = homeAddress;
        Description = description;
    }
}
