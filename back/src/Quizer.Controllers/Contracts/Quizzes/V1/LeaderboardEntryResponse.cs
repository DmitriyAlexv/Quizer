namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Запись в таблице лидеров по квизу.
/// </summary>
public record LeaderboardEntryResponse(
    Guid UserId,
    string UserName,
    int Score,
    DateTime CompletedAt);
