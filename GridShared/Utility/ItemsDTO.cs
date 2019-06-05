using GridShared.Pagination;
using GridShared.Totals;
using System.Collections.Generic;

namespace GridShared.Utility
{
    public class ItemsDTO<T>
    {
        public IEnumerable<T> Items { get; set; }
        public TotalsDTO Totals { get; set; }
        public PagerDTO Pager { get; set; }

        public ItemsDTO()
        {
        }

        public ItemsDTO(IEnumerable<T> items, TotalsDTO totals, PagerDTO pager)
        {
            Items = items;
            Totals = totals;
            Pager = pager;
        }
    }
}
