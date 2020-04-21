using GridShared.Filtering;
using System;

namespace GridShared.Events
{
    public class FilterEventArgs : EventArgs
    {
        public IFilterColumnCollection FilteredColumns { get; set; }
    }

    public class FilterEventCancelArgs : EventArgs
    {
        public bool Cancel { get; set; } = false;
    }
}
