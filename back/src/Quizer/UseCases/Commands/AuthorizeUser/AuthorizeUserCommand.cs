using MediatR;

namespace Quizer.UseCases.Commands.AuthorizeUser;

public record AuthorizeUserCommand(string Email, string Password) : IRequest<string>;
