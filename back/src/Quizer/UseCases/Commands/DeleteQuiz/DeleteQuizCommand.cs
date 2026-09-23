using MediatR;

namespace Quizer.UseCases.Commands.DeleteQuiz;

public record DeleteQuizCommand(Guid Id) : IRequest;
