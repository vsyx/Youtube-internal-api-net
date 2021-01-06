using System.Net.Http;
using YoutubeApi.Interfaces;
using System;

namespace YoutubeApi
{
    public class YoutubeApiConfig : IYoutubeApiConfig
    {
        public HttpClient Client { get; private set; }
        public Func<HttpRequestMessage, HttpRequestMessage>? PrepareHttpRequestMessage { get; private set; } = null;

        public YoutubeApiConfig(HttpClient client)
        {
            Client = client;
        }

        public YoutubeApiConfig(
                HttpClient client,
                Func<HttpRequestMessage, HttpRequestMessage> prepareHttpRequestMessage)
        {
            Client = client;
            PrepareHttpRequestMessage = prepareHttpRequestMessage;
        }
    }
}
