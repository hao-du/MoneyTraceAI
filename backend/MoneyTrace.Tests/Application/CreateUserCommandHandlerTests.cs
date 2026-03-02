using FluentAssertions;
using MoneyTrace.Application.Data;
using MoneyTrace.Application.Users.Commands;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;
using NSubstitute;

namespace MoneyTrace.Tests.Application;

public class CreateUserCommandHandlerTests
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _userRepository = Substitute.For<IRepository<User>>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new CreateUserCommandHandler(_userRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnSuccessWithGuid()
    {
        // Arrange
        var command = new CreateUserCommand("john.doe", "securepassword123", "John Doe", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldAddUserToRepository()
    {
        // Arrange
        var command = new CreateUserCommand("jane.doe", "securepassword123", "Jane Doe", "A description");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userRepository.Received(1).Add(Arg.Is<User>(u => u.Username == "jane.doe" && u.FullName == "Jane Doe"));
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldSaveChanges()
    {
        // Arrange
        var command = new CreateUserCommand("user123", "pass", "User 123", null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmptyUsername_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateUserCommand("", "pass", "Full Name", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.UsernameRequired");
        _userRepository.DidNotReceive().Add(Arg.Any<User>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
