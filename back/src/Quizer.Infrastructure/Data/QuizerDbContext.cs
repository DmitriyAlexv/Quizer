using Microsoft.EntityFrameworkCore;
using Quizer.QuizAggregate.Entities;


namespace Quizer.Infrastructure.Data;

/// <summary>
/// Контекст базы данных приложения.
/// </summary>
public class QuizerDbContext(DbContextOptions<QuizerDbContext> options) : DbContext(options)
{
    public DbSet<QuizAggregate.Quiz> Quizzes => Set<QuizAggregate.Quiz>();

    public DbSet<UserAggregate.User> Users => Set<UserAggregate.User>();

    public DbSet<Question> Questions => Set<Question>();

    public DbSet<Answer> Answers => Set<Answer>();

    public DbSet<Attempt> Attempts => Set<Attempt>();

    public DbSet<AttemptAnswer> AttemptAnswers => Set<AttemptAnswer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(QuizerDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
