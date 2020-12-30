using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace YoutubeApi.Util
{
    public class RegexUtils
    {
        public static string CombinePatterns(string[] patterns) => $"(?:{String.Join('|', patterns)})";

        public static string ExtractFirstValidGroupMatch(GroupCollection groups, int matchSkipCount) => groups.Values
            .Skip(matchSkipCount)
            .Select(g => g.Value)
            .FirstOrDefault(str => !string.IsNullOrEmpty(str)) ?? String.Empty;
    }
}
