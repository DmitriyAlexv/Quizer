using MediatR;

namespace Quizer.UseCases.Queries.GetAttemptResult;

public record GetAttemptResultQuery(Guid QuizId, Guid AttemptId) : IRequest<AttemptResultDto?>;
