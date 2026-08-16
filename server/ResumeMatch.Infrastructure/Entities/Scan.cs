namespace ResumeMatch.Infrastructure.Entities;

public class Scan
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? ResumeFilename { get; set; }

    public string? JobTitle { get; set; }

    public decimal MatchScore { get; set; }

    public DateTime CreatedAt { get; set; }
}