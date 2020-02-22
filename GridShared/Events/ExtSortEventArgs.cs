using GridShared.Sorting;
using System;

namespace GridShared.Events
{
    public class ExtSortEventArgs : EventArgs
    {
        public DefaultOrderColumnCollection SortValues { get; set; }
    }
}
