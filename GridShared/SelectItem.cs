using System.Runtime.Serialization;

namespace GridShared
{
    [DataContract]
    public class SelectItem
    {
        public const string ListFilter = "ListFilter";

        [DataMember(Order = 1)]
        public string Value { get; set; }

        [DataMember(Order = 2)]
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
