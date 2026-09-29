using System.Reflection;
using Microsoft.OpenApi.Models;
using Quizer;
using Quizer.Abstractions.Auth;
using Quizer.Controllers;
using Quizer.Infrastructure.Auth;
using Quizer.Infrastructure.Data;
using Quizer.Infrastructure.Identity;
using Quizer.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets("02e57bcf-ebc7-4b27-b2cf-4b2473fdc067");

// Add services to the container.
builder.Services.AddQuizer();

builder.Services.AddPostgresSqlStorage(builder.Configuration["App:DbConnectionString"]!);
builder.Services.AddIdentityPostgresSqlStorage(builder.Configuration["App:IdentityDbConnectionString"]!);

var jwtOptionsSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtOptions>(jwtOptionsSection);
builder.Services.AddAuthenticationScheme(jwtOptionsSection.Get<JwtOptions>()!);

builder.Services.AddControllersLayer();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Quizer API",
        Version = "v1",
        Description = "API для платформы квизов: создание квизов, вопросов, ответов, прохождение попыток и получение результатов.",
        Contact = new OpenApiContact
        {
            Name = "Quizer",
        },
    });

    // Подключение XML-документации
    var xmlFiles = new[]
    {
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml",
        $"{typeof(Quizer.Controllers.Entry).Assembly.GetName().Name}.xml",
        $"{typeof(Quizer.Entry).Assembly.GetName().Name}.xml",
    };

    foreach (var xmlFile in xmlFiles)
    {
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    }

    // Схема авторизации JWT Bearer
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите JWT-токен, полученный при авторизации. Формат: Bearer {token}",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            },
            Array.Empty<string>()
        },
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Quizer API v1");
        options.RoutePrefix = "swagger";
    });
    app.UseMiddleware<DisableCorsMiddleware>();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
