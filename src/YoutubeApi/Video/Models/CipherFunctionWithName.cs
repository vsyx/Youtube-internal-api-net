using System;

namespace YoutubeApi.Video.Models
{
    public struct CipherFunctionWithName
    {
        public string Name { get; }
        public Func<char[], int, char[]> CipherFunction { get; }

        public CipherFunctionWithName(string name, Func<char[], int, char[]> cipherFunction)
        {
            Name = name;
            CipherFunction = cipherFunction;
        }
    }
}
