using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Domain.Entities;

public class User : Entity
{
    private User() { } // EF Core

    private User(Guid id, string username, string passwordHash, string fullName, string? description)
        : base(id)
    {
        Username = username;
        PasswordHash = passwordHash;
        FullName = fullName;
        Description = description;
    }

    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public static Result<User> Create(string username, string passwordHash, string fullName, string? description)
    {
        if (string.IsNullOrWhiteSpace(username))
            return Result.Failure<User>(new Error("User.UsernameRequired", "Username is required."));

        return new User(Guid.CreateVersion7(), username, passwordHash, fullName, description);
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void UpdateProfile(string fullName, string? description)
    {
        FullName = fullName;
        Description = description;
    }
}
