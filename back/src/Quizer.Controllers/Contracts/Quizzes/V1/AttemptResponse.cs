using Quizer.QuizAggregate.Enums;

namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Ответ с данными попытки прохождения квиза.
/// </summary>
public record AttemptResponse(
    Guid Id,
    Guid QuizId,
    Guid UserId,
    AttemptStatus Status,
    DateTime StartedAt,
    DateTime? CompletedAt,
    IReadOnlyCollection<AttemptAnswerResponse> Answers);
