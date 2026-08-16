using ResumeMatch.Core.Models;
using ResumeMatch.Core.Services;
using Microsoft.EntityFrameworkCore;
using ResumeMatch.Infrastructure.Data;
using ResumeMatch.Infrastructure.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSingleton<KeywordMatchingService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/api/health", () =>
{
    return Results.Ok(new
    {
        message = "Resume Match API is working"
    });
});

app.MapPost("/api/scan", async (
    ScanRequest request,
    KeywordMatchingService matchingService,
    AppDbContext db) =>
{
    var userExists = await db.Users
        .AnyAsync(user => user.Id == request.UserId);

    if (!userExists)
    {
        return Results.BadRequest(new
        {
            message = "User does not exist"
        });
    }

    var result = matchingService.Compare(
        request.ResumeText,
        request.JobText
    );

    var scan = new Scan
    {
        Id = Guid.NewGuid(),
        UserId = request.UserId,
        ResumeFilename = request.ResumeFilename,
        JobTitle = request.JobTitle,
        MatchScore = result.MatchScore,
        CreatedAt = DateTime.UtcNow
    };

    db.Scans.Add(scan);

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        scanId = scan.Id,
        matchScore = result.MatchScore,
        matchedKeywords = result.MatchedKeywords,
        missingKeywords = result.MissingKeywords
    });
});

app.MapGet("/api/db-health", async (AppDbContext db) =>
{
    var canConnect = await db.Database.CanConnectAsync();

    if (canConnect)
    {
        return Results.Ok(new
        {
            message = "Database connection is working"
        });
    }

    return Results.Problem("Database connection failed");
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
