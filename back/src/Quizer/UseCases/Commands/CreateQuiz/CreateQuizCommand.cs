using MediatR;

namespace Quizer.UseCases.Commands.CreateQuiz;

public record CreateQuizCommand(string Title, string Description, Guid OwnerId) : IRequest<Guid>;
