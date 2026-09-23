using MediatR;
using Quizer.QuizAggregate;

namespace Quizer.UseCases.Queries.GetLeaderboard;

public class GetLeaderboardQueryHandler(IQuizRepository quizRepository)
    : IRequestHandler<GetLeaderboardQuery, IReadOnlyList<LeaderboardEntry>>
{
    public async Task<IReadOnlyList<LeaderboardEntry>> Handle(GetLeaderboardQuery request, CancellationToken cancellationToken)
    {
        return await quizRepository.GetLeaderboardAsync(request.QuizId, request.Limit, cancellationToken);
    }
}
