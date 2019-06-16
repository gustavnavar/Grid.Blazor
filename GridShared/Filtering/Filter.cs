namespace GridShared.Filtering
{
    public class  Filter
    { 
        public string Type { set; get; }

        public string Value { set; get; }

        public Filter()
        {
        }

        public Filter(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}