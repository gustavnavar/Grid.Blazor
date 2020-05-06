using System.Collections.Generic;

namespace GridShared.Filtering
{
    public class FilterCollection : List<Filter>
    {
        public FilterCollection() : base()
        {
        }

        public FilterCollection(string  type, string value) : base()
        {
            Add(new Filter(type, value));
        }

        public FilterCollection(Filter[] filters) : base()
        {
            foreach (var filter in filters)
            {
                if(!string.IsNullOrWhiteSpace(filter.Type))
                    Add(filter.Type, filter.Value);
            }   
        }

        public FilterCollection(IEnumerable<Filter> filters) : base()
        {
            foreach (var filter in filters)
            {
                if (!string.IsNullOrWhiteSpace(filter.Type))
                    Add(filter.Type, filter.Value);
            }
        }

        public void Add(string type, string value)
        {
            Add(new Filter(type, value));
        }
    }
}