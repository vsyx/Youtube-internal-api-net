using System.Net.Http;
using System.Collections.Generic;

namespace YoutubeApi
{
    public class YoutubeApiConfig
    {
        public HttpClient Client { get; private set; }
        public Dictionary<string, string> Headers { get; set; }

        public YoutubeApiConfig(HttpClient client)
        {
            Client = client;
            Headers = new Dictionary<string, string>();
        }

        public YoutubeApiConfig(HttpClient client, string youtubeCookie)
        {
            Client = client;
            Headers = new Dictionary<string, string>()
            {
                { "Cookie", youtubeCookie }
            };
        }
    }
}
