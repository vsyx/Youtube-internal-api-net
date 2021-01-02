using System.Text.RegularExpressions;
using YoutubeApi.Exceptions;
using YoutubeApi.Util;

namespace YoutubeApi.Video
{
    public class VideoDetailsConfigExtractor
    {
        static readonly Regex[] VideoConfigRegexes = new Regex[]
        {
            new Regex(@"var\sytInitialPlayerResponse\s?=\s?({.+?});"),
            new Regex(@";ytplayer\.config\s?=\s?({.+?});")
        };

        public static string Extract(string videoHtmlPage, bool returnDocument = false)
        {
            foreach(var regex in VideoConfigRegexes)
            {
                var match = regex.Match(videoHtmlPage);

                if (!match.Success)
                {
                    continue;
                }

                var startIndex = match.Groups[1].Index;
                var endBracketIndex = JsonUtils.FindEndingJsonBracket(videoHtmlPage, startIndex);

                if (endBracketIndex == -1)
                {
                    continue;
                }

                int endIndex = endBracketIndex - startIndex + 1;
                string configJsonText = videoHtmlPage.Substring(startIndex, endIndex);

                return configJsonText;
                //return JsonDocument.Parse(configJsonText);
            }

            throw new ExtractionException("Unable to extract video config");
        }
    }
}
