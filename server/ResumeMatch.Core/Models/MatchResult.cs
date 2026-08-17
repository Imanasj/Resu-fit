namespace ResumeMatch.Core.Models;

public class MatchResult
{
    public int MatchScore { get; set; }

    public List<string> MatchedKeywords { get; set; } = new List<string>();

    public List<string> MissingKeywords { get; set; } = new List<string>();
}