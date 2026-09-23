using Quizer;
using Quizer.Controllers;
using Quizer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddQuizer();
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("App:DbConnectionString")!);
builder.Services.AddControllersLayer();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();