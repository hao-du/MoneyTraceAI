using MediatR;
using MoneyTrace.Application.Data;
using MoneyTrace.Domain.Entities;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Application.Users.Commands;

internal sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IRepository<User> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Simple hashing for MVP. In reality, use BCrypt or similar.
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var userResult = User.Create(request.Username, passwordHash, request.FullName, request.Description);

        if (userResult.IsFailure)
        {
            return Result.Failure<Guid>(userResult.Error);
        }

        _userRepository.Add(userResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(userResult.Value.Id);
    }
}
