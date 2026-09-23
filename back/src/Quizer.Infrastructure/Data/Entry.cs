using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quizer.Abstractions.Data;
using Quizer.Infrastructure.Data.Quiz.Repository;
using Quizer.Infrastructure.Data.User.Repository;
using Quizer.QuizAggregate;
using Quizer.UserAggregate;

namespace Quizer.Infrastructure.Data;

/// <summary>
/// Регистрация зависимостей для БД уровня инфраструктуры.
/// </summary>
public static class Entry
{
    public static IServiceCollection AddPostgresSqlStorage(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<QuizerDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IQuizRepository, QuizRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
