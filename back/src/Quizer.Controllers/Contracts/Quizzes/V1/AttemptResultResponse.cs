using Quizer.QuizAggregate.Enums;

namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Результат прохождения квиза.
/// </summary>
/// <param name="AttemptId">Идентификатор попытки.</param>
/// <param name="QuizId">Идентификатор квиза.</param>
/// <param name="UserId">Идентификатор пользователя, проходящего квиз.</param>
/// <param name="Status">Статус попытки.</param>
/// <param name="StartedAt">Дата и время начала попытки.</param>
/// <param name="CompletedAt">Дата и время завершения попытки (null, если попытка не завершена).</param>
/// <param name="TotalPoints">Максимальное количество баллов за квиз.</param>
/// <param name="EarnedPoints">Количество набранных баллов.</param>
/// <param name="Questions">Список результатов по каждому вопросу.</param>
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
/// <param name="QuestionId">Идентификатор вопроса.</param>
/// <param name="Text">Текст вопроса.</param>
/// <param name="Type">Тип вопроса.</param>
/// <param name="Points">Количество баллов за вопрос.</param>
/// <param name="IsCorrect">Признак того, является ли ответ правильным.</param>
/// <param name="TextAnswer">Текстовый ответ пользователя (для вопросов с открытым ответом).</param>
/// <param name="SelectedAnswerIds">Список идентификаторов выбранных вариантов ответа.</param>
public record QuestionResultResponse(
    Guid QuestionId,
    string Text,
    QuestionType Type,
    int Points,
    bool IsCorrect,
    string? TextAnswer,
    IReadOnlyCollection<Guid> SelectedAnswerIds);
