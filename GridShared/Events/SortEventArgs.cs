using GridShared.Sorting;
using System;

namespace GridShared.Events
{
    public class SortEventArgs : EventArgs
    {
        public string ColumnName { get; set; }
        public GridSortDirection Direction { get; set; }
    }
}
