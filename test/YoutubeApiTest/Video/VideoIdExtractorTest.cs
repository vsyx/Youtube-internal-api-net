using NUnit.Framework;
using YoutubeApi.Video;

namespace YoutubeApiTest.Video
{
    [TestFixture]
    public class VideoIdExtractorTest
    {
        const string VideoUrlRegular = @"https://www.youtube.com/watch?v=jNQXAC9IVRw";
        const string VideoUrlShort = @"https://youtu.be/jNQXAC9IVRw";
        const string VideoId = @"jNQXAC9IVRw";

        [Test]
        public void TestVideoUrlRegular()
        {
            Assert.AreEqual(VideoId, VideoIdExtractor.Extract(VideoUrlRegular));
        }

        [Test]
        public void TestUrlShort()
        {
            Assert.AreEqual(VideoId, VideoIdExtractor.Extract(VideoUrlShort));
        }

        [Test]
        public void TestVideoId()
        {
            Assert.AreEqual(VideoId, VideoIdExtractor.Extract(VideoId));
        }

        // TODO fail-cases (not that important, but still)
    }
}
