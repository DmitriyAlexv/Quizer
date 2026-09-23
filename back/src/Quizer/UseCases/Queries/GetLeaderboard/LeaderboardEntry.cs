namespace Quizer.UseCases.Queries.GetLeaderboard;

/// <summary>
/// Запись в таблице лидеров по квизу.
/// </summary>
public record LeaderboardEntry(Guid UserId, string UserName, int Score, DateTime CompletedAt);
