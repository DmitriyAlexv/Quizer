using MediatR;

namespace Quizer.UseCases.Commands.RegisterUser;

public record RegisterUserCommand(string Email, string Password, string Name) : IRequest<Guid>;
