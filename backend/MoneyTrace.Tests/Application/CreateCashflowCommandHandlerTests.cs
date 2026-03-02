using FluentAssertions;
using MoneyTrace.Application.Data;
using MoneyTrace.Application.Transactions.Commands;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;
using NSubstitute;

namespace MoneyTrace.Tests.Application;

public class CreateCashflowCommandHandlerTests
{
    private readonly IRepository<Cashflow> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateCashflowCommandHandler _handler;

    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _currencyId = Guid.NewGuid();

    public CreateCashflowCommandHandlerTests()
    {
        _repository = Substitute.For<IRepository<Cashflow>>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new CreateCashflowCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_IncomeCommand_ShouldReturnSuccessWithId()
    {
        // Arrange
        var command = new CreateCashflowCommand(
            _userId, DateTime.UtcNow, 2500m, _currencyId, "Monthly Salary", "salary,income", IsIncome: true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_IncomeCommand_ShouldAddCashflowToRepository()
    {
        // Arrange
        var command = new CreateCashflowCommand(
            _userId, DateTime.UtcNow, 100m, _currencyId, "Groceries", null, IsIncome: false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repository.Received(1).Add(Arg.Is<Cashflow>(c =>
            c.UserId == _userId &&
            c.Amount == 100m &&
            c.IsIncome == false));
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCallSaveChanges()
    {
        // Arrange
        var command = new CreateCashflowCommand(
            _userId, DateTime.UtcNow, 50m, _currencyId, null, null, IsIncome: true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_BothDirections_ShouldSucceed(bool isIncome)
    {
        var command = new CreateCashflowCommand(
            _userId, DateTime.UtcNow, 200m, _currencyId, "Test", null, isIncome);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
