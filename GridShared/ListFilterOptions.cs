using System;

namespace GridShared
{
    public class ListFilterOptions
    {
        public bool IncludeIsNull { get; set; }
        public bool IncludeIsNotNull { get; set; }
        public bool ShowSelectAllButtons { get; set; }
        public bool ShowSearchInput { get; set; }
        public StringComparison SearchComparisonMethod { get; set; } = StringComparison.CurrentCultureIgnoreCase;
        public int SearchInputDebounceMilliseconds { get; set; } = 300;
    }
}
