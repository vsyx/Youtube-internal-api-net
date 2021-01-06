using System.Text.Json;
using System.Threading.Tasks;
using YoutubeApi.Exceptions;
using YoutubeApi.Interfaces;
using YoutubeApi.Video.Models;

namespace YoutubeApi.Video
{
    public class VideoConfigDeserializer
    {
        private IYoutubeApiConfig YoutubeApiConfig { get; set; }
        private StreamingDataDecoder StreamingDataDecoder { get; set; }

        public VideoConfigDeserializer(IYoutubeApiConfig youtubeApiConfig)
        {
            YoutubeApiConfig = youtubeApiConfig;
            StreamingDataDecoder = new StreamingDataDecoder(YoutubeApiConfig);
        }

        public async Task<VideoConfig> Deserialize(string videoHtmlPage, string videoConfigJsonText)
        {
            var decodedConfig = await StreamingDataDecoder.Decode(videoHtmlPage, videoConfigJsonText);

            using var jsonDocument = JsonDocument.Parse(decodedConfig);
            var root = jsonDocument.RootElement;

            var videoDetails = root.GetProperty("videoDetails").ToString();
            var streamingData = root.GetProperty("streamingData").ToString();

            if (videoDetails == null || streamingData == null)
            {
                throw new ExtractionException();
            }

            return new VideoConfig()
            {
                VideoDetails = JsonSerializer.Deserialize<VideoDetails>(videoDetails),
                StreamingData = JsonSerializer.Deserialize<StreamingData>(streamingData)
            };
        }
    }
}
