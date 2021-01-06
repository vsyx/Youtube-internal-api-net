using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using YoutubeApi.Exceptions;
using YoutubeApi.Interfaces;
using YoutubeApi.Video.Models;

namespace YoutubeApi.Video
{
    public class VideoConfigExtractor
    {
        public IYoutubeApiConfig YoutubeConfig { get; private set; }
        public StreamingDataDecoder Decoder { get; private set; }

        public VideoConfigExtractor(IYoutubeApiConfig youtubeApiConfig) 
        {
            this.YoutubeConfig = youtubeApiConfig;
            this.Decoder = new StreamingDataDecoder(youtubeApiConfig.Client);
        }

        public async Task<VideoConfig> ExtractAsync(string stringContainingId)
        {
            string videoId = VideoIdExtractor.Extract(stringContainingId);
            string videoHtmlPage = await FetchWithUserCookies($"https://www.youtube.com/watch?v={videoId}");
            string configJsonText = VideoDetailsConfigExtractor.Extract(videoHtmlPage);
            string decodedConfigJsonText = await Decoder.Decode(videoHtmlPage, configJsonText);

            return ParseVideoConfig(decodedConfigJsonText);
        }

        public VideoConfig ParseVideoConfig(string videoConfig)
        {
            using var jsonDocument = JsonDocument.Parse(videoConfig);
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

        private async Task<string> FetchWithUserCookies(string uri)
        {
            using var message = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri)
            };
            YoutubeConfig.PrepareHttpRequestMessage?.Invoke(message);

            using var response = await YoutubeConfig.Client.SendAsync(message);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
