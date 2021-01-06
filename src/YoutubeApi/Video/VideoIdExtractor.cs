using System;
using System.Linq;
using System.Text.RegularExpressions;
using YoutubeApi.Exceptions;

namespace YoutubeApi.Video
{
    public class VideoIdExtractor
    {
        private const string VideoIdPattern = @"[0-9A-Za-z_-]{10}[048AEIMQUYcgkosw]";
        private static readonly Regex[] VideoIdRegexes = new string[] {
            $"^(?:https?://)?(?:www\\.?)?(?:m\\.?)?youtube\\.com/.*?v=({VideoIdPattern})", // regular url
            $"^(?:https?://)?youtu\\.be/({VideoIdPattern})$", // short url
            $"^({VideoIdPattern})$" // just id
        }.Select(pattern => new Regex(pattern)).ToArray();

        public static string Extract(string stringContainingId)
        {
            try
            {
                return VideoIdRegexes
                    .Select(regex => regex.Match(stringContainingId))
                    .Where(match => match.Success)
                    .First().Groups[1].Value;
            }
            catch (InvalidOperationException)
            {
                throw new ExtractionException($"Can't extract videoId from {stringContainingId}");
            }
        }
    }
}
