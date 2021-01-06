using System;
using System.Net.Http;

namespace YoutubeApi.Interfaces
{
    public interface IYoutubeApiConfig
    {
        HttpClient Client { get; }
        Func<HttpRequestMessage, HttpRequestMessage>? PrepareHttpRequestMessage { get; } 
    }
}
