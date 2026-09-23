using MediatR;

namespace Quizer.UseCases.Commands.PublishQuiz;

public record PublishQuizCommand(Guid Id) : IRequest;
