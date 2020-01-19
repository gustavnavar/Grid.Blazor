using GridShared.Filtering;
using System;

namespace GridShared.Events
{
    public class FilterEventArgs : EventArgs
    {
        public IFilterColumnCollection FilteredColumns { get; set; }
    }
}
