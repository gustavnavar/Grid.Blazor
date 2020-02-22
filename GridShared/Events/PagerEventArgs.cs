using GridShared.Pagination;
using System;

namespace GridShared.Events
{
    public class PagerEventArgs : EventArgs
    {
        public PagerDTO Pager { get; set; }
    }
}
