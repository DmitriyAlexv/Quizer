namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Ответ с данными ответа пользователя на вопрос.
/// </summary>
public record AttemptAnswerResponse(
    Guid Id,
    Guid QuestionId,
    string? TextAnswer,
    IReadOnlyCollection<Guid> SelectedAnswerIds,
    bool IsCorrect);
