namespace ResumeMatch.Infrastructure.Entities;

public class ScanRedFlag
{
    public Guid Id { get; set; }

    public Guid ScanId { get; set; }

    public string FlagType { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}