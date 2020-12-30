using System;
using System.Text.RegularExpressions;
using YoutubeApi.Util;
using YoutubeApi.Exceptions;

namespace YoutubeApi.Video
{
    public class VideoIdExtractor
    {
        private const string VideoIdPattern = @"[0-9A-Za-z_-]{10}[048AEIMQUYcgkosw]";
        private static readonly string[] VideoIdPatterns = {
            $"^(?:https?://)?(?:www\\.?)?(?:m\\.?)?youtube\\.com/.*?v=({VideoIdPattern})", // regular url
            $"^(?:https?://)?youtu\\.be/({VideoIdPattern})$", // short url
            $"^({VideoIdPattern})$" // just id
        };

        private static readonly Lazy<Regex> VideoIdRegex = new Lazy<Regex>(() =>
            new Regex(RegexUtils.CombinePatterns(VideoIdPatterns), RegexOptions.Compiled));

        public static string Extract(string stringContainingId)
        {
            var match = VideoIdRegex.Value.Match(stringContainingId);
            var videoId = RegexUtils.ExtractFirstValidGroupMatch(match.Groups, 1);

            if (videoId == String.Empty)
            {
                throw new ExtractionException($"Can't extract videoId from {stringContainingId}");
            }

            return videoId;
        }
    }
}
