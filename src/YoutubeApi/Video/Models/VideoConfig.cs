using System.Text.Json;

namespace YoutubeApi.Video.Models
{
    public class VideoConfig
    {
        public StreamingData? StreamingData { get; set; }
        public VideoDetails? VideoDetails { get; set; }
        public JsonDocument? Rest { get; set; }
    }
}
