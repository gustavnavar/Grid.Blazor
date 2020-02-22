namespace GridShared
{
    public class SelectItem
    {
        public const string ListFilter = "ListFilter";

        public string Value { get; set; }

        public string Title { get; set; }

        public SelectItem()
        { }

        public SelectItem(string value, string title)
        {
            Value = value;
            Title = title;
        }
    }
}
