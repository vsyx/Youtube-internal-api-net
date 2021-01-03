using System;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using YoutubeApi.Video;
using YoutubeApi;

namespace YoutubeApiTest.Video
{
    [TestFixture]
    public class VideoConfigExtractorTest
    {
        #nullable disable
        private HttpClient Client { get; set; }
        #nullable enable

        [SetUp]
        public void Setup()
        {
            Client = new HttpClient();
        }

        [TearDown]
        public void Dispose()
        {
            Client?.Dispose();
        }

        [Test]
        public async Task TestSignedVideo()
        {
            var config = new YoutubeApiConfig(Client);
            var extractor = new VideoConfigExtractor(config);
            var result = await extractor.ExtractAsync("UxxajLWwzqY");

            // TODO make a **partial** request for the video (ideally, only downloading a small amount of bytes)
            // to see if we can actually download it
            var url = result.StreamingData?.AdaptiveFormats[0].Url;

            Assert.AreNotEqual(String.Empty, url);
        }
    }
}
