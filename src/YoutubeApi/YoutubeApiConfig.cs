using System.Net.Http;

namespace YoutubeApi
{
    public class YoutubeApiConfig
    {
        public HttpClient Client { get; private set; }
        public string YoutubeCookie { get; set; }

        public YoutubeApiConfig(HttpClient client)
        {
            this.Client = client;
            this.YoutubeCookie = "";
        }

        public YoutubeApiConfig(HttpClient client, string youtubeCookie)
        {
            this.Client = client;
            this.YoutubeCookie = youtubeCookie;
        }
    }
}
