using Quizer.QuizAggregate.Enums;

namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Результат прохождения квиза.
/// </summary>
public record AttemptResultResponse(
    Guid AttemptId,
    Guid QuizId,
    Guid UserId,
    AttemptStatus Status,
    DateTime StartedAt,
    DateTime? CompletedAt,
    int TotalPoints,
    int EarnedPoints,
    IReadOnlyCollection<QuestionResultResponse> Questions);

/// <summary>
/// Результат ответа на конкретный вопрос.
/// </summary>
public record QuestionResultResponse(
    Guid QuestionId,
    string Text,
    QuestionType Type,
    int Points,
    bool IsCorrect,
    string? TextAnswer,
    IReadOnlyCollection<Guid> SelectedAnswerIds);
