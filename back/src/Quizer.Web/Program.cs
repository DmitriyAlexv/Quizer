using Quizer;
using Quizer.Controllers;
using Quizer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets("02e57bcf-ebc7-4b27-b2cf-4b2473fdc067");

// Add services to the container.
builder.Services.AddQuizer();
builder.Services.AddInfrastructure(builder.Configuration["App:DbConnectionString"]!);
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