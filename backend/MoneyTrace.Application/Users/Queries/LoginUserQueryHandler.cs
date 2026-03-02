using MediatR;
using MoneyTrace.Application.Authentication;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Users.Queries;

internal sealed class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, Result<string>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IJwtProvider _jwtProvider;

    public LoginUserQueryHandler(IRepository<User> userRepository, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<string>> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        var user = users.FirstOrDefault(u => u.Username == request.Username);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Result.Failure<string>(new Error("Authentication.Failed", "Invalid username or password."));
        }

        string token = _jwtProvider.GenerateToken(user);
        return Result.Success(token);
    }
}
