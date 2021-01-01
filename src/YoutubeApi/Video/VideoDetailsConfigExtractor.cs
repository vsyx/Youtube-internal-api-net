using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using YoutubeApi.Exceptions;
using YoutubeApi.Util;
using YoutubeApi.Video.Models;
using YoutubeApi.Extensions;

namespace YoutubeApi.Video
{
    public class VideoDetailsConfigExtractor
    {
        static readonly (Regex, Func<JsonElement, VideoConfig>)[] VideoConfigRegexes = 
            new (Regex, Func<JsonElement, VideoConfig>)[]
        {
            (new Regex(@"var\sytInitialPlayerResponse\s?=\s?({.+?});"), (json) => new VideoConfig()
             {
                VideoDetails = json.GetProperty("videoDetails").ToObject<VideoDetails>(),
                StreamingData = json.GetProperty("streamingData").ToObject<StreamingData>()
             }),
            // TODO handle this properly (json is messed up)
            (new Regex(@";ytplayer\.config\s?=\s?({.+?});"), (json) =>
                 throw new ExtractionException("Deserialization matching not implemented")) 
        };

        public static VideoConfig Extract(string videoHtmlPage, bool returnDocument = false)
        {
            foreach(var (regex, deserializer) in VideoConfigRegexes)
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

                JsonDocument document = JsonDocument.Parse(configJsonText);
                JsonElement root = document.RootElement;

                var config = deserializer(root);

                if (returnDocument) 
                {
                    config.Rest = document;
                }
                else
                {
                    document.Dispose();
                }

                return config;
            }

            throw new ExtractionException("Unable to extract video config");
        }
    }
}
