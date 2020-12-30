using System;

namespace YoutubeApi.Exceptions
{
    [Serializable]
    public class ExtractionException : Exception
    {
        public ExtractionException() : base() { }
        public ExtractionException(string message) : base(message) { }
        public ExtractionException(string message, Exception inner) : base(message, inner) { }

        protected ExtractionException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
