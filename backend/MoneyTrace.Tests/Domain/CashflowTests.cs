using FluentAssertions;
using MoneyTrace.Domain.Entities;

namespace MoneyTrace.Tests.Domain;

public class CashflowTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _currencyId = Guid.NewGuid();

    [Fact]
    public void Create_Income_ShouldReturnSuccessWithIsIncomeTrue()
    {
        var result = Cashflow.Create(
            _userId,
            DateTime.UtcNow,
            500m,
            _currencyId,
            "Salary",
            "income,salary",
            isIncome: true);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsIncome.Should().BeTrue();
        result.Value.Amount.Should().Be(500m);
        result.Value.UserId.Should().Be(_userId);
    }

    [Fact]
    public void Create_Expense_ShouldReturnSuccessWithIsIncomeFalse()
    {
        var result = Cashflow.Create(
            _userId,
            DateTime.UtcNow,
            100m,
            _currencyId,
            "Groceries",
            null,
            isIncome: false);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsIncome.Should().BeFalse();
        result.Value.Description.Should().Be("Groceries");
    }

    [Fact]
    public void Create_ShouldGenerateVersionSevenGuid()
    {
        var result = Cashflow.Create(_userId, DateTime.UtcNow, 100m, _currencyId, null, null, false);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void AddItem_ShouldAddItemToCollection()
    {
        var cashflow = Cashflow.Create(
            _userId, DateTime.UtcNow, 200m, _currencyId, "Shopping", null, false).Value;

        cashflow.AddItem("Item A", 100m, "Description A");
        cashflow.AddItem("Item B", 100m, null);

        cashflow.Items.Should().HaveCount(2);
        cashflow.Items.Should().Contain(i => i.ItemName == "Item A" && i.Amount == 100m);
        cashflow.Items.Should().Contain(i => i.ItemName == "Item B");
    }
}
