using MediatR;

namespace Quizer.UseCases.Commands.CompleteAttempt;

public record CompleteAttemptCommand(Guid QuizId, Guid AttemptId) : IRequest;
