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

        services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<QuizerIdentityDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}