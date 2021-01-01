using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable
namespace YoutubeApi.Video.Models
{
    public class Thumbnail2 {
        [JsonPropertyName("url")]
        public string Url { get; set; } 

        [JsonPropertyName("width")]
        public int Width { get; set; } 

        [JsonPropertyName("height")]
        public int Height { get; set; } 
    }

    public class Thumbnail {
        [JsonPropertyName("thumbnails")]
        public List<Thumbnail2> Thumbnails { get; set; } 
    }

    public class VideoDetails {
        [JsonPropertyName("videoId")]
        public string VideoId { get; set; } 

        [JsonPropertyName("title")]
        public string Title { get; set; } 

        [JsonPropertyName("lengthSeconds")]
        public string LengthSeconds { get; set; } 

        [JsonPropertyName("isLive")]
        public bool IsLive { get; set; } 

        [JsonPropertyName("keywords")]
        public List<string> Keywords { get; set; } 

        [JsonPropertyName("channelId")]
        public string ChannelId { get; set; } 

        [JsonPropertyName("isOwnerViewing")]
        public bool IsOwnerViewing { get; set; } 

        [JsonPropertyName("shortDescription")]
        public string ShortDescription { get; set; } 

        [JsonPropertyName("isCrawlable")]
        public bool IsCrawlable { get; set; } 

        [JsonPropertyName("isLiveDvrEnabled")]
        public bool IsLiveDvrEnabled { get; set; } 

        [JsonPropertyName("thumbnail")]
        public Thumbnail Thumbnail { get; set; } 

        [JsonPropertyName("liveChunkReadahead")]
        public int LiveChunkReadahead { get; set; } 

        [JsonPropertyName("averageRating")]
        public double AverageRating { get; set; } 

        [JsonPropertyName("allowRatings")]
        public bool AllowRatings { get; set; } 

        [JsonPropertyName("viewCount")]
        public string ViewCount { get; set; } 

        [JsonPropertyName("author")]
        public string Author { get; set; } 

        [JsonPropertyName("isLowLatencyLiveStream")]
        public bool IsLowLatencyLiveStream { get; set; } 

        [JsonPropertyName("isPrivate")]
        public bool IsPrivate { get; set; } 

        [JsonPropertyName("isUnpluggedCorpus")]
        public bool IsUnpluggedCorpus { get; set; } 

        [JsonPropertyName("latencyClass")]
        public string LatencyClass { get; set; } 

        [JsonPropertyName("isLiveContent")]
        public bool IsLiveContent { get; set; } 
    }
}
