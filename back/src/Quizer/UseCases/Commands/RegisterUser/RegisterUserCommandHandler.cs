using MediatR;
using Quizer.Abstractions.Auth;
using Quizer.Abstractions.Data;
using Quizer.UserAggregate;

namespace Quizer.UseCases.Commands.RegisterUser;

public class RegisterUserCommandHandler(
    IIdentityService identityService,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<RegisterUserCommand, Guid>
{
    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var userId = await identityService.RegisterAsync(request.Email, request.Password, cancellationToken);

        var user = new User(userId, request.Name);
        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return userId;
    }
}
