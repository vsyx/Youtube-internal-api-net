using System;
using System.Net.Http;
using System.Threading.Tasks;
using YoutubeApi.Interfaces;
using YoutubeApi.Video.Models;

namespace YoutubeApi.Video
{
    public class VideoConfigExtractor
    {
        public IYoutubeApiConfig YoutubeConfig { get; private set; }
        private VideoConfigDeserializer Deserializer { get; set; }

        public VideoConfigExtractor(IYoutubeApiConfig youtubeApiConfig) 
        {
            YoutubeConfig = youtubeApiConfig;
            Deserializer = new VideoConfigDeserializer(YoutubeConfig);
        }

        public async Task<VideoConfig> ExtractAsync(string stringContainingId)
        {
            string videoId = VideoIdExtractor.Extract(stringContainingId);
            string videoHtmlPage = await FetchWithUserCookies($"https://www.youtube.com/watch?v={videoId}");
            string configJsonText = VideoDetailsConfigExtractor.Extract(videoHtmlPage);

            return await Deserializer.Deserialize(videoHtmlPage, configJsonText);
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
