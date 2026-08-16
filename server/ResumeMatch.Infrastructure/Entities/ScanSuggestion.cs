namespace ResumeMatch.Infrastructure.Entities;

public class ScanSuggestion
{
    public Guid Id { get; set; }

    public Guid ScanId { get; set; }

    public string Suggestion { get; set; } = string.Empty;
}