namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Запись в таблице лидеров по квизу.
/// </summary>
/// <param name="UserId">Идентификатор пользователя.</param>
/// <param name="UserName">Имя пользователя.</param>
/// <param name="Score">Количество набранных баллов.</param>
/// <param name="CompletedAt">Дата и время завершения попытки.</param>
public record LeaderboardEntryResponse(
    Guid UserId,
    string UserName,
    int Score,
    DateTime CompletedAt);
