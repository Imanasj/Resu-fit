using ResumeMatch.Core.Models;
using ResumeMatch.Core.Services;
using Microsoft.EntityFrameworkCore;
using ResumeMatch.Infrastructure.Data;
using ResumeMatch.Infrastructure.Entities;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("ResuFitDb"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSingleton<KeywordMatchingService>();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

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
    var userId = request.UserId == Guid.Empty ? Guid.NewGuid() : request.UserId;
    var userExists = await db.Users.AnyAsync(user => user.Id == userId);

    if (!userExists)
    {
        db.Users.Add(new User
        {
            Id = userId,
            Email = $"local-user-{userId}@example.com",
            PasswordHash = "local-dev-user",
            CreatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }

    var result = matchingService.Compare(
        request.ResumeText,
        request.JobText
    );

    var scan = new Scan
    {
        Id = Guid.NewGuid(),
        UserId = userId,
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
