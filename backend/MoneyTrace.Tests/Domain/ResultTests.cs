using FluentAssertions;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Tests.Domain;

public class ResultTests
{
    [Fact]
    public void Success_ShouldReturnSuccessResult()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void SuccessOfT_ShouldReturnResultWithValue()
    {
        var result = Result.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void Failure_ShouldReturnFailureResult()
    {
        var error = new Error("Test.Error", "Something went wrong");

        var result = Result.Failure(error);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void FailureOfT_AccessingValue_ShouldThrowInvalidOperationException()
    {
        var error = new Error("Test.Error", "Something went wrong");
        var result = Result.Failure<string>(error);

        var act = () => _ = result.Value;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void SuccessOfT_WithSameErrorAsNone_ShouldSucceed()
    {
        // Result.Success<T> always uses Error.None internally — verify no exception
        var act = () => Result.Success("value");

        act.Should().NotThrow();
    }

    [Fact]
    public void FailureOfT_WithNullValueType_ShouldReturnDefault()
    {
        var error = new Error("Test.Error", "Something went wrong");
        var result = Result.Failure<int>(error);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Test.Error");
    }
}
