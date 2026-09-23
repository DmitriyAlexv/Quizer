using MediatR;
using Quizer.QuizAggregate.Entities;

namespace Quizer.UseCases.Commands.CreateAttempt;

public record CreateAttemptCommand(Guid QuizId, Guid UserId) : IRequest<Attempt>;
