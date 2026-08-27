using ResumeMatch.Core.Models;
using System.Text.RegularExpressions;

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
            .Where(skill => ContainsSkill(jobText, skill))
            .ToList();

        var matchedKeywords = jobKeywords
            .Where(skill => ContainsSkill(resumeText, skill))
            .ToList();

        var missingKeywords = jobKeywords
            .Where(skill => !ContainsSkill(resumeText, skill))
            .ToList();

        int score = jobKeywords.Count == 0
            ? 0
            : (int)Math.Round(
                (double)matchedKeywords.Count / jobKeywords.Count * 100);

        return new MatchResult
        {
            MatchScore = score,
            MatchedKeywords = matchedKeywords,
            MissingKeywords = missingKeywords
        };
    }

    private static bool ContainsSkill(string text, string skill)
    {
        var pattern =
            $@"(?<![A-Za-z0-9]){Regex.Escape(skill)}(?![A-Za-z0-9])";

        return Regex.IsMatch(
            text,
            pattern,
            RegexOptions.IgnoreCase);
    }
}