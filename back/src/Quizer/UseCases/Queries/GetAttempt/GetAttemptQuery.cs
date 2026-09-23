using MediatR;
using Quizer.QuizAggregate.Entities;

namespace Quizer.UseCases.Queries.GetAttempt;

public record GetAttemptQuery(Guid QuizId, Guid AttemptId) : IRequest<Attempt?>;
