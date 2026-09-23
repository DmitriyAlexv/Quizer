using Microsoft.Extensions.DependencyInjection;

namespace Quizer;

/// <summary>
/// Регистрация зависимостей уровня домена и приложения.
/// </summary>
public static class Entry
{
    public static IServiceCollection AddQuizer(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Entry).Assembly));
        
        return services;
    }
}
