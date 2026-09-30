using Microsoft.OpenApi;

namespace Quizer.Web.Swagger;

public static class Entry
{
    public static IServiceCollection AddSwagger(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Quizer API",
                Version = "v1",
                Description =
                    "API для платформы квизов: создание квизов, вопросов, ответов, прохождение попыток и получение результатов.",
                Contact = new OpenApiContact
                {
                    Name = "Quizer",
                },
            });
            
            // Подключение XML-документации
            var xmlControllers = $"{typeof(Quizer.Controllers.Entry).Assembly.GetName().Name}.xml";
            var xmlControllersPath = Path.Combine(AppContext.BaseDirectory, xmlControllers);
            if (File.Exists(xmlControllersPath))
            {
                options.IncludeXmlComments(xmlControllersPath);
            }
            
            // Схема авторизации JWT Bearer
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Введите JWT-токен, полученный при авторизации. Формат: {token}",
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer", document),
                    []
                }
            });
        });
    }
}