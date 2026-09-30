using Quizer;
using Quizer.Abstractions.Auth;
using Quizer.Controllers;
using Quizer.Infrastructure.Auth;
using Quizer.Infrastructure.Data;
using Quizer.Infrastructure.Identity;
using Quizer.Web.Middleware;
using Quizer.Web.Swagger;

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
builder.Services.AddSwagger();

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
