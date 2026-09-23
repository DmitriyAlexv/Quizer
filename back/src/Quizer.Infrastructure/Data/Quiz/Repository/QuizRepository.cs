using Microsoft.EntityFrameworkCore;
using Quizer.Abstractions.Pagination;
using Quizer.QuizAggregate;
using Quizer.QuizAggregate.Entities;
using Quizer.QuizAggregate.Enums;
using Quizer.UseCases.Queries.GetLeaderboard;


namespace Quizer.Infrastructure.Data.Quiz.Repository;

public class QuizRepository(QuizerDbContext dbContext) : IQuizRepository
{
    public async Task<QuizAggregate.Quiz?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Quizzes
            .Include(q => q.Questions)
                .ThenInclude(q => q.Answers)
            .Include(q => q.Attempts)
                .ThenInclude(a => a.Answers)
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
    }

    public async Task<PagedResult<QuizAggregate.Quiz>> GetListAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Quizzes.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(q => q.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<QuizAggregate.Quiz>(items, totalCount, page, pageSize);
    }

    public async Task<PagedResult<Question>> GetQuestionsAsync(Guid quizId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Questions
            .AsNoTracking()
            .Where(q => EF.Property<Guid>(q, "QuizId") == quizId);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Include(q => q.Answers)
            .OrderBy(q => q.Order)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Question>(items, totalCount, page, pageSize);
    }

    public async Task<Question?> GetQuestionAsync(Guid quizId, Guid questionId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Questions
            .AsNoTracking()
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == questionId && EF.Property<Guid>(q, "QuizId") == quizId, cancellationToken);
    }

    public async Task<PagedResult<Attempt>> GetAttemptsAsync(Guid quizId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Attempts
            .AsNoTracking()
            .Where(a => a.QuizId == quizId);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Include(a => a.Answers)
            .OrderByDescending(a => a.StartedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Attempt>(items, totalCount, page, pageSize);
    }

    public async Task<Attempt?> GetAttemptAsync(Guid quizId, Guid attemptId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Attempts
            .AsNoTracking()
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a => a.Id == attemptId && a.QuizId == quizId, cancellationToken);
    }

    public async Task<IReadOnlyList<LeaderboardEntry>> GetLeaderboardAsync(Guid quizId, int limit, CancellationToken cancellationToken = default)
    {
        var completedAttempts = await dbContext.Attempts
            .AsNoTracking()
            .Where(a => a.QuizId == quizId && a.Status == AttemptStatus.Completed)
            .Include(a => a.Answers)
            .ToListAsync(cancellationToken);

        var questions = await dbContext.Questions
            .AsNoTracking()
            .Where(q => EF.Property<Guid>(q, "QuizId") == quizId)
            .ToListAsync(cancellationToken);

        var questionPoints = questions.ToDictionary(q => q.Id, q => q.Points);

        var entries = new List<LeaderboardEntry>();
        foreach (var attempt in completedAttempts)
        {
            var score = attempt.Answers
                .Where(a => a.IsCorrect)
                .Sum(a => questionPoints.GetValueOrDefault(a.QuestionId, 0));

            var user = await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == attempt.UserId, cancellationToken);

            entries.Add(new LeaderboardEntry(
                attempt.UserId,
                user?.Name ?? string.Empty,
                score,
                attempt.CompletedAt ?? attempt.StartedAt));
        }

        return entries
            .OrderByDescending(e => e.Score)
            .ThenBy(e => e.CompletedAt)
            .Take(limit)
            .ToList();
    }

    public async Task AddAsync(QuizAggregate.Quiz quiz, CancellationToken cancellationToken = default)
    {
        await dbContext.Quizzes.AddAsync(quiz, cancellationToken);
    }

    public void Update(QuizAggregate.Quiz quiz)
    {
        dbContext.Quizzes.Update(quiz);
    }

    public void Delete(QuizAggregate.Quiz quiz)
    {
        dbContext.Quizzes.Remove(quiz);
    }
}
