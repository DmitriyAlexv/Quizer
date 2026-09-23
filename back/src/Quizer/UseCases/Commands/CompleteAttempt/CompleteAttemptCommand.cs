using MediatR;

namespace Quizer.UseCases.Commands.CompleteAttempt;

public record CompleteAttemptCommand(Guid QuizId, Guid AttemptId, Guid UserId) : IRequest;
