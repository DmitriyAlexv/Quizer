using MediatR;

namespace Quizer.UseCases.Commands.UpdateQuiz;

public record UpdateQuizCommand(Guid Id, string Title, string Description) : IRequest;
