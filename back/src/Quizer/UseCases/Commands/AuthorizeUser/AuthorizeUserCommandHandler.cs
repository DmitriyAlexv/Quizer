using MediatR;
using Quizer.Abstractions.Auth;

namespace Quizer.UseCases.Commands.AuthorizeUser;

public class AuthorizeUserCommandHandler(
    IIdentityService identityService) : IRequestHandler<AuthorizeUserCommand, string>
{
    public async Task<string> Handle(AuthorizeUserCommand request, CancellationToken cancellationToken)
    {
        return await identityService.AuthorizeAsync(request.Email, request.Password, cancellationToken);
    }
}
