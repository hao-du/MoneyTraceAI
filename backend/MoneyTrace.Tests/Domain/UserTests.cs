using FluentAssertions;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Tests.Domain;

public class UserTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnSuccessResult()
    {
        // Arrange
        var username = "john.doe";
        var passwordHash = "hashed_password";
        var fullName = "John Doe";

        // Act
        var result = User.Create(username, passwordHash, fullName, null);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Username.Should().Be(username);
        result.Value.FullName.Should().Be(fullName);
        result.Value.Id.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyUsername_ShouldReturnFailureResult(string? username)
    {
        // Act
        var result = User.Create(username!, "hash", "Full Name", null);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.UsernameRequired");
    }

    [Fact]
    public void Create_ShouldGenerateUniqueIds()
    {
        // Act
        var user1 = User.Create("user1", "hash", "User One", null);
        var user2 = User.Create("user2", "hash", "User Two", null);

        // Assert
        user1.Value.Id.Should().NotBe(user2.Value.Id);
    }
}
