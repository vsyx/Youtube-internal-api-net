using System;
using System.Web;
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
            var sig = (isUrlDecoded) ? signature : HttpUtility.UrlDecode(signature);

            var sigArray = sig.ToCharArray();

            foreach (var transformFunction in TransformFunctions)
            {
                sigArray = CipherFunctions[transformFunction.Name](sigArray, transformFunction.Arg);
            }

            return new string(sigArray);
        }
    }
}
