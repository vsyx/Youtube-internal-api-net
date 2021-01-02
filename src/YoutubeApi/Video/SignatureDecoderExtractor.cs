using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeApi.Exceptions;
using YoutubeApi.Video.Models;

namespace YoutubeApi.Video
{
    public class SignatureDecoderExtractor
    {
        public static readonly Regex[] FunctionPatterns = new string[]
        {
            "\\b[cs]\\s*&&\\s*[adf]\\.set\\([^,]+\\s*,\\s*encodeURIComponent\\s*\\(\\s*([a-zA-Z0-9$]+)\\(",
            "\\b[a-zA-Z0-9]+\\s*&&\\s*[a-zA-Z0-9]+\\.set\\([^,]+\\s*,\\s*encodeURIComponent\\s*\\(\\s*([a-zA-Z0-9$]+)\\(",
            "(?:\\b|[^a-zA-Z0-9$])([a-zA-Z0-9$]{2})\\s*=\\s*function\\(\\s*a\\s*\\)\\s*\\{\\s*a\\s*=\\s*a\\.split\\(\\s*\"\"\\s*\\)",
            "([a-zA-Z0-9$]+)\\s*=\\s*function\\(\\s*a\\s*\\)\\s*\\{\\s*a\\s*=\\s*a\\.split\\(\\s*\"\"\\s*\\)",
            "([\"'])signature\\1\\s*,\\s*([a-zA-Z0-9$]+)\\(",
            "\\.sig\\|\\|([a-zA-Z0-9$]+)\\(",
            "yt\\.akamaized\\.net/\\)\\s*\\|\\|\\s*.*?\\s*[cs]\\s*&&\\s*[adf]\\.set\\([^,]+\\s*,\\s*(?:encodeURIComponent\\s*\\()?\\s*()$",
            "\\b[cs]\\s*&&\\s*[adf]\\.set\\([^,]+\\s*,\\s*([a-zA-Z0-9$]+)\\(",
            "\\b[a-zA-Z0-9]+\\s*&&\\s*[a-zA-Z0-9]+\\.set\\([^,]+\\s*,\\s*([a-zA-Z0-9$]+)\\(",
            "\\bc\\s*&&\\s*a\\.set\\([^,]+\\s*,\\s*\\([^)]*\\)\\s*\\(\\s*([a-zA-Z0-9$]+)\\(",
            "\\bc\\s*&&\\s*[a-zA-Z0-9]+\\.set\\([^,]+\\s*,\\s*\\([^)]*\\)\\s*\\(\\s*([a-zA-Z0-9$]+)\\("
        }.Select(p => new Regex(p)).ToArray();

        public static readonly (Regex, Func<char[], int, char[]>)[] CipherFunctions = new (string, Func<char[], int, char[]>)[]
        {
            (@"\{\w\.reverse\(\)\}", (charArray, _) => charArray.Reverse().ToArray()),
            (@"\{\w\.splice\(0,\w\)\}", (c, deleteCount) => 
             {
                var doubleDeleteCount = deleteCount * 2;
                var end = doubleDeleteCount + (c.Length - doubleDeleteCount);

                char[] spliced = new char[c.Length - deleteCount];
                Array.Copy(c, 0, spliced, 0, deleteCount);
                Array.Copy(c, doubleDeleteCount, spliced, deleteCount, spliced.Length - deleteCount);

                return spliced;
             }),
            (@"\{var\s\w=\w\[0];\w\[0]=\w\[\w%\w.length];\w\[\w]=\w\}", (charArray, position) =>
             {
                char c = charArray[0];
                charArray[0] = charArray[position % charArray.Length];
                charArray[position] = c;

                return charArray;
             }),
             (@"\{var\s\w=\w\[0];\w\[0]=\w\[\w%\w.length];\w\[\w%\w.length]=\w\}", (charArray, position) =>
              {
                char c = charArray[0];
                int positionModulo = position % charArray.Length;

                charArray[0] = charArray[positionModulo];
                charArray[positionModulo] = c;

                return charArray;
              })
        }.Select(p => (new Regex(p.Item1), p.Item2)).ToArray();

        static readonly Regex NormalizeFunctionName = new Regex("[^$A-Za-z0-9_]");

        public HttpClient Client { get; private set; }

        public SignatureDecoderExtractor(HttpClient client) 
        {
            Client = client;
        }

        // TODO cache decoders
        public async Task<SignatureDecoder> ExtractDecoder(string baseJsUrl)
        {
            var baseJs = await Client.GetStringAsync(baseJsUrl);

            var functionName = GetSignatureFunctionName(baseJs);
            var transformMatch = Regex.Match(baseJs, Regex.Escape(functionName)
                    + "=function\\(\\w\\)\\{[a-z=\\.\\(\\\\\"\\)]*;(.*);(?:.+)\\}");

            if (!transformMatch.Success)
            {
                throw new ExtractionException("Coudln't extract transformMatch functions");
            }

            var splittedTransformMatch = transformMatch.Groups[1].Value
                .Split(';');

            var cipherFunctionVariableName = splittedTransformMatch[0].Split('.')[0];
            var transformFunctions = splittedTransformMatch
                .Select(func => ExtractFunctionNameAndArg(func))
                .ToList();

            var cipherFunctions = ExtractCipherFunctions(cipherFunctionVariableName, baseJs);
            
            return new SignatureDecoder(transformFunctions, cipherFunctions);
        }

        private Dictionary<string, Func<char[], int, char[]>> ExtractCipherFunctions(string variableName, string baseJs)
        {
            var replacedFuncVariableName = Regex.Escape(NormalizeFunctionName.Replace(variableName, String.Empty));
            var match = Regex.Match(
                    baseJs,
                    "var " + replacedFuncVariableName + "=\\{(.*?)\\};",
                    RegexOptions.Singleline);

            if (match.Success)
            {
                var func = Regex.Replace(match.Groups[1].Value, "\n", " ").Split(", ");

                return func
                    .Select(f => f.Split(':', 2))
                    .ToDictionary(n => n[0], f => GetCipherFunction(f[1]));
            }

            throw new ExtractionException("Couldn't extract the transform object");
        }

        private Func<char[], int, char[]> GetCipherFunction(string jsFunction)
        {
            foreach (var (regex, cipherFunction) in CipherFunctions)
            {
                if (regex.Match(jsFunction).Success)
                {
                    return cipherFunction;
                }
            }

            throw new ExtractionException("Couldn't find a corresponding cipher function");
        }

        private string GetSignatureFunctionName(string baseJs)
        {
            foreach (var regex in FunctionPatterns)
            {
                var match = regex.Match(baseJs);

                if (match.Success)
                {
                    var matchedValue = match.Groups[1].Value;
                    return NormalizeFunctionName.Replace(matchedValue, String.Empty);
                }
            }

            throw new ExtractionException("Couldn't extract signature function name");
        }

        private TransformFunction ExtractFunctionNameAndArg(string jsFunction)
        {
            var match = Regex.Match(jsFunction, @"\w+\.(\w+)\(\w,(\d+)\)");
            if (match.Success)
            {
                var groups = match.Groups;

                var name = groups[1].Value;
                var arg = Int32.Parse(groups[2].Value);

                return new TransformFunction(name, arg);
            }

            throw new ExtractionException("Coudln't extract the function name and or argument");
        }
    }
}
