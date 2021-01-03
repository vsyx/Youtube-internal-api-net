# Youtube-internal-api-net 

**Inernal** YouTube api wrapper for downloading video metadata. This library web scrapes the data just like youtube-dl does.

# Usage example
```csharp
var config = new YoutubeApiConfig(Client);
var extractor = new VideoConfigExtractor(config);

// can be either the full/short url or just the videoId
var stringContainingVideoId = "https://www.youtube.com/watch?v=jNQXAC9IVRw" 
var result = await extractor.ExtractAsync(stringContainingVideoId);
Console.WriteLine(result?.VideoDetails.Title);
```

# Disclaimer

By using this library you're breaking YouTube's terms of service (see [here](https://www.youtube.com/static?template=terms)).

# Thanks

* https://github.com/ytdl-org/youtube-dl 
* https://github.com/sealedtx/java-youtube-downloader
