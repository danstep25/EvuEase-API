using Microsoft.EntityFrameworkCore;
using EvuEase.Api.Configuration;
using EvuEase.Application;
using EvuEase.Infrastructure;
using EvuEase.Infrastructure.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure CORS
builder.AddCorsConfiguration();

//Dependency Injection
builder.Services
    .AddInfrastructure()
    .AddServices();

//Database Connection
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// Use CORS
app.UseCorsConfiguration();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Map OpenAPI document endpoint
    app.MapOpenApi();
    
    // Map Scalar API Reference UI at /scalar
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("EvuEase API")
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
    
    // Redirect root to Scalar in development
    app.MapGet("/", () => Results.Redirect("/scalar"));
}

app.MapControllers();
app.Run();

