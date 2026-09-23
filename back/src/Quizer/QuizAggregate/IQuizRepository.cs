using Quizer.Abstractions.Pagination;
using Quizer.QuizAggregate.Entities;
using Quizer.UseCases.Queries.GetLeaderboard;

namespace Quizer.QuizAggregate;

/// <summary>
/// Репозиторий для работы с квизами.
/// </summary>
public interface IQuizRepository
{
    Task<Quiz?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<Quiz>> GetListAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    Task<PagedResult<Question>> GetQuestionsAsync(Guid quizId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<Question?> GetQuestionAsync(Guid quizId, Guid questionId, CancellationToken cancellationToken = default);

    Task<PagedResult<Attempt>> GetAttemptsAsync(Guid quizId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<Attempt?> GetAttemptAsync(Guid quizId, Guid attemptId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeaderboardEntry>> GetLeaderboardAsync(Guid quizId, int limit, CancellationToken cancellationToken = default);

    Task AddAsync(Quiz quiz, CancellationToken cancellationToken = default);

    void Update(Quiz quiz);

    void Delete(Quiz quiz);
}
