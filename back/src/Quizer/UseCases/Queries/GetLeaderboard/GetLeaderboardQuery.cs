using MediatR;

namespace Quizer.UseCases.Queries.GetLeaderboard;

public record GetLeaderboardQuery(Guid QuizId, int Limit = 10) : IRequest<IReadOnlyList<LeaderboardEntry>>;
