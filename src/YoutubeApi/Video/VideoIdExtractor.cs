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

        private static readonly Regex VideoIdRegex = new Regex(RegexUtils.CombinePatterns(VideoIdPatterns));

        public static string Extract(string stringContainingId)
        {
            var match = VideoIdRegex.Match(stringContainingId);
            try
            {
                var videoIdGroup = RegexUtils.ExtractFirstValidGroupMatch(match.Groups, 1);
                return videoIdGroup.Value;
            }
            catch (InvalidOperationException)
            {
                throw new ExtractionException($"Can't extract videoId from {stringContainingId}");
            }
        }
    }
}
