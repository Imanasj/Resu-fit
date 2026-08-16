namespace ResumeMatch.Core.Models;

public class ScanRequest
{
    public Guid UserId { get; set; }

    public string ResumeText { get; set; } = string.Empty;

    public string JobText { get; set; } = string.Empty;

    public string? ResumeFilename { get; set; }

    public string? JobTitle { get; set; }
}