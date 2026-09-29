namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Ответ с данными ответа пользователя на вопрос.
/// </summary>
/// <param name="Id">Идентификатор ответа пользователя.</param>
/// <param name="QuestionId">Идентификатор вопроса.</param>
/// <param name="TextAnswer">Текстовый ответ пользователя (для вопросов с открытым ответом).</param>
/// <param name="SelectedAnswerIds">Список идентификаторов выбранных вариантов ответа.</param>
/// <param name="IsCorrect">Признак того, является ли ответ правильным.</param>
public record AttemptAnswerResponse(
    Guid Id,
    Guid QuestionId,
    string? TextAnswer,
    IReadOnlyCollection<Guid> SelectedAnswerIds,
    bool IsCorrect);
