using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Quizer.Controllers.Filters;

namespace Quizer.Controllers;

/// <summary>
/// Регистрация зависимостей уровня контроллеров.
/// </summary>
public static class Entry
{
    public static IServiceCollection AddControllersLayer(this IServiceCollection services)
    {
        services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddValidatorsFromAssembly(typeof(Entry).Assembly);

        return services;
    }
}
