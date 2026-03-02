namespace MoneyTrace.Application.Authentication;

public interface IJwtProvider
{
    string GenerateToken(Domain.Entities.User user);
}
