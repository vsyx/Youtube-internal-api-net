namespace YoutubeApi.Video.Models
{
    public struct TransformFunction
    {
        public string Name { get; }
        public int Arg { get; }

        public TransformFunction(string name, int arg)
        {
            Name = name;
            Arg = arg;
        }
    }
}
