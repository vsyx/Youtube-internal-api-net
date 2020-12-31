using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace YoutubeApi.Util
{
    public class RegexUtils
    {
        public static string CombinePatterns(string[] patterns) => $"(?:{String.Join('|', patterns)})";

        public static Group ExtractFirstValidGroupMatch(GroupCollection groups, int matchSkipCount) => groups.Values
            .Skip(matchSkipCount)
            .First(g => !string.IsNullOrEmpty(g.Value));
    }
}
