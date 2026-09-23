using Quizer.QuizAggregate.Enums;

namespace Quizer.UseCases.Queries.GetAttemptResult;

/// <summary>
/// Результат прохождения квиза.
/// </summary>
public record AttemptResultDto
{
    public Guid AttemptId { get; init; }

    public Guid QuizId { get; init; }

    public Guid UserId { get; init; }

    public AttemptStatus Status { get; init; }

    public DateTime StartedAt { get; init; }

    public DateTime? CompletedAt { get; init; }

    public int TotalPoints { get; init; }

    public int EarnedPoints { get; init; }

    public IReadOnlyList<QuestionResultDto> Questions { get; init; } = [];
}

/// <summary>
/// Результат ответа на конкретный вопрос.
/// </summary>
public record QuestionResultDto
{
    public Guid QuestionId { get; init; }

    public string Text { get; init; } = string.Empty;

    public QuestionType Type { get; init; }

    public int Points { get; init; }

    public bool IsCorrect { get; init; }

    public string? TextAnswer { get; init; }

    public IReadOnlyList<Guid> SelectedAnswerIds { get; init; } = [];
}
