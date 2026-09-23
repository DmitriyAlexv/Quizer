using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quizer.Abstractions.Data;
using Quizer.Infrastructure.Data;
using Quizer.Infrastructure.Data.Quiz.Repository;
using Quizer.Infrastructure.Data.User.Repository;
using Quizer.QuizAggregate;
using Quizer.UserAggregate;

namespace Quizer.Infrastructure;

/// <summary>
/// Регистрация зависимостей уровня инфраструктуры.
/// </summary>
public static class Entry
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<QuizerDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IQuizRepository, QuizRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
