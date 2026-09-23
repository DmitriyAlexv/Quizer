using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Quizer.Controllers;

/// <summary>
/// Регистрация зависимостей уровня контроллеров.
/// </summary>
public static class Entry
{
    public static IServiceCollection AddControllersLayer(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        
        return services;
    }
}
