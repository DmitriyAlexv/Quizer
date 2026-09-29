using Quizer.QuizAggregate.Enums;

namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Ответ с данными попытки прохождения квиза.
/// </summary>
/// <param name="Id">Идентификатор попытки.</param>
/// <param name="QuizId">Идентификатор квиза.</param>
/// <param name="UserId">Идентификатор пользователя, проходящего квиз.</param>
/// <param name="Status">Статус попытки.</param>
/// <param name="StartedAt">Дата и время начала попытки.</param>
/// <param name="CompletedAt">Дата и время завершения попытки (null, если попытка не завершена).</param>
/// <param name="Answers">Список ответов пользователя на вопросы.</param>
public record AttemptResponse(
    Guid Id,
    Guid QuizId,
    Guid UserId,
    AttemptStatus Status,
    DateTime StartedAt,
    DateTime? CompletedAt,
    IReadOnlyCollection<AttemptAnswerResponse> Answers);
