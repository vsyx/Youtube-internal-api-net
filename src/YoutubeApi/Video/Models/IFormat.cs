#nullable disable
namespace YoutubeApi.Video.Models
{
    public interface IFormat {
        public int Itag { get; set; } 
        public string Url { get; set; } 
        public string MimeType { get; set; } 
        public int Bitrate { get; set; } 
        public int Width { get; set; } 
        public int Height { get; set; } 
        public string LastModified { get; set; } 
        public string ContentLength { get; set; } 
        public string Quality { get; set; } 
        public int Fps { get; set; } 
        public string QualityLabel { get; set; } 
        public string ProjectionType { get; set; } 
        public int AverageBitrate { get; set; } 
        public string ApproxDurationMs { get; set; } 
        public string AudioSampleRate { get; set; } 
        public int AudioChannels { get; set; } 
    }
}
