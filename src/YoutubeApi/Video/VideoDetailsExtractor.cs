using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace YoutubeApi.Video
{
    public class VideoDetailsExtractor
    {
        public YoutubeApiConfig YoutubeConfig { get; private set; }

        public VideoDetailsExtractor(YoutubeApiConfig youtubeApiConfig) 
        {
            this.YoutubeConfig = youtubeApiConfig;
        }

        public async Task ExtractAsync(string stringContainingId)
        {
            string videoId = VideoIdExtractor.Extract(stringContainingId);
            string videoHtmlPage = await FetchVideoPage(GetVideoPageUrl(videoId));
        }

        private async Task<string> FetchVideoPage(string uri)
        {
            using var message = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri)
            };
            YoutubeConfig.SetupMessage(message);

            using var response = await YoutubeConfig.Client.SendAsync(message);
            return await response.Content.ReadAsStringAsync();
        }

        private string GetVideoPageUrl(string videoId) => $"https://www.youtube.com/watch?v={videoId}";
    }
}
