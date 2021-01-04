using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using YoutubeApi.Video.Models;

namespace YoutubeApi.Video
{
    public class SignatureDecoder
    {
        public List<TransformFunction> TransformFunctions { get; }
        public Dictionary<string, Func<char[], int, char[]>> CipherFunctions { get; }

        public SignatureDecoder(List<TransformFunction> transformFunctions, Dictionary<string, Func<char[], int, char[]>> cipherFunctions)
        {
            TransformFunctions = transformFunctions;
            CipherFunctions = cipherFunctions;
        }

        public string Decode(string signature, bool isUrlDecoded = false)
        {
            var urlDecodedSignature = (isUrlDecoded) ? signature : HttpUtility.UrlDecode(signature);

            return TransformFunctions.Aggregate(
                    urlDecodedSignature.ToCharArray(), 
                    (signatureArray, transformFunction) 
                        => CipherFunctions[transformFunction.Name](signatureArray, transformFunction.Arg),
                    decipheredSignature => new string(decipheredSignature));
        }
    }
}
