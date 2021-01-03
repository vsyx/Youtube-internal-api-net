using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections;

using YoutubeApi.Exceptions;
using YoutubeApi.Util;
using System.Collections.Concurrent;

namespace YoutubeApi.Video
{
    public class StreamingDataDecoder
    {
        public HttpClient Client { get; private set; }
        public SignatureDecoderExtractor SignatureDecoderExtractor { get; private set; }
        public ConcurrentDictionary<string, Lazy<Task<SignatureDecoder>>> SignatureDecoderCache { get; private set; }

        public StreamingDataDecoder(HttpClient client)
        {
            Client = client;
            SignatureDecoderExtractor = new SignatureDecoderExtractor(Client);
            SignatureDecoderCache = new ConcurrentDictionary<string, Lazy<Task<SignatureDecoder>>>();
        }

        public async Task<string> Decode(string videoHtmlPage, string configJsonText)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonHashtableConverter());
            var videoConfig = JsonSerializer.Deserialize<Hashtable>(configJsonText, options);

            var streamingData = videoConfig?["streamingData"] as Hashtable;

            var formats = (streamingData?["formats"] as Object[])?.Cast<Hashtable>().ToList();
            var adaptiveFormats = (streamingData?["adaptiveFormats"] as Object[])?.Cast<Hashtable>().ToList();

            if (streamingData != null)
            {
                if (formats != null)
                {
                    streamingData["formats"] = await DecipherFormats(formats, videoHtmlPage);
                }
                if (adaptiveFormats != null)
                {
                    streamingData["adaptiveFormats"] = await DecipherFormats(adaptiveFormats, videoHtmlPage);
                }
            }

            return JsonSerializer.Serialize(videoConfig);
        }

        private async Task<List<Hashtable>> DecipherFormats(
                List<Hashtable> formats,
                string videoHtmlPage)
        {
            var decodedFormats = new List<Hashtable>();
            var cipheredFormats = new List<Hashtable>();

            foreach (var format in formats)
            {
                if (format.ContainsKey("signatureCipher"))
                {
                    var signatureCipher = format["signatureCipher"] as string;
                    signatureCipher?.Split('&')
                        .Select(pair => pair.Split('='))
                        .ToList()
                        .ForEach(keyAndValue => format.Add(keyAndValue[0], keyAndValue[1]));
                }

                var undecodedUrl = format["url"] as string;
                if (undecodedUrl != null)
                {
                    format["url"] = HttpUtility.UrlDecode(undecodedUrl);
                }

                var url = format["url"] as string ?? String.Empty;

                if (url.Contains("signature") 
                        || (!format.ContainsKey("s")
                            && (url.Contains("&sig=") || url.Contains("&lsig="))
                            )
                        )
                {
                    decodedFormats.Add(format);
                    continue;
                }
                cipheredFormats.Add(format);
            }

            if (cipheredFormats.Count != 0)
            {
                var baseJsUrl = ExtractBaseJsUrl(videoHtmlPage);

                // TODO add exception handling
                // wrapped within a Lazy to prevent multiple invocations
                var signatureDecoderFunc = SignatureDecoderCache.GetOrAdd(baseJsUrl, new Lazy<Task<SignatureDecoder>>(() => SignatureDecoderExtractor.ExtractDecoder(baseJsUrl)));

                var signatureDecoder = await signatureDecoderFunc.Value;

                var uncipheredFormats = cipheredFormats.Select(format =>
                {
                    format["url"] = (format["url"] as string)
                        + "&sig="
                        + signatureDecoder.Decode(format["s"] as string ?? String.Empty);
                    return format;
                });

                decodedFormats.AddRange(uncipheredFormats);
            }

            return decodedFormats;
        }

        static readonly Regex BaseJsUrlRegex = new Regex("\"PLAYER_JS_URL\":\"(.*?base\\.js)\"");
        public static string ExtractBaseJsUrl(string videoHtmlPage)
        {
            var match = BaseJsUrlRegex.Match(videoHtmlPage);
            if (match.Success)
            {
                return "https://youtube.com" + match.Groups[1].Value;
            }

            throw new ExtractionException("Unable to extract base js url");
        }
    }
}
