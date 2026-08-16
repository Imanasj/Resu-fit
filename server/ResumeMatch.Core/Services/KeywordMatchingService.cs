using ResumeMatch.Core.Models;

namespace ResumeMatch.Core.Services;

public class KeywordMatchingService
{
    private readonly string[] _skills =
    {
        "c#",
        "java",
        "python",
        "react",
        "angular",
        "javascript",
        "typescript",
        "html",
        "css",
        "sql",
        "postgresql",
        "git",
        "github",
        "asp.net",
        ".net"
    };

    public MatchResult Compare(string resumeText, string jobText)
    {
        var jobKeywords = _skills
            .Where(skill => jobText.Contains(skill, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var matchedKeywords = jobKeywords
            .Where(skill => resumeText.Contains(skill, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var missingKeywords = jobKeywords
            .Where(skill => !resumeText.Contains(skill, StringComparison.OrdinalIgnoreCase))
            .ToList();

        int score = jobKeywords.Count == 0
            ? 0
            : (int)Math.Round((double)matchedKeywords.Count / jobKeywords.Count * 100);

        return new MatchResult
        {
            MatchScore = score,
            MatchedKeywords = matchedKeywords,
            MissingKeywords = missingKeywords
        };
    }
}