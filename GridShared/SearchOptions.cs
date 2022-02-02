namespace GridShared
{
    public class SearchOptions
    {
        public bool Enabled { get; set; } = true;
        public bool OnlyTextColumns { get; set; } = true;
        public bool HiddenColumns { get; set; } = false;
        public bool SplittedWords { get; set; } = false;
    }
}
