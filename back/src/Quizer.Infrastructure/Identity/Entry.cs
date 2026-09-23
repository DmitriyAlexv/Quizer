using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quizer.Abstractions.Auth;

namespace Quizer.Infrastructure.Identity;

/// <summary>
/// Регистрация зависимостей для БД аутентификации уровня инфраструктуры.
/// </summary>
public static class Entry
{
    public static IServiceCollection AddIdentityPostgresSqlStorage(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<QuizerIdentityDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<QuizerIdentityDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}