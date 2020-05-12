using GridShared.Pagination;
using GridShared.Totals;
using System.Collections.Generic;

namespace GridShared.Utility
{
    public class ItemsDTO<T>
    {
        public IEnumerable<T> Items { get; set; }
        public IEnumerable<T> OriginalItems { get; }
        public IEnumerable<T> AllFilteredItems { get; }
        public TotalsDTO Totals { get; set; }
        public PagerDTO Pager { get; set; }

        public ItemsDTO()
        {
        }

        public ItemsDTO(IEnumerable<T> items, TotalsDTO totals, PagerDTO pager, 
            IEnumerable<T> originalItems, IEnumerable<T> allFilteredItems)
        {
            Items = items;
            Totals = totals;
            Pager = pager;
            OriginalItems = originalItems;
            AllFilteredItems = allFilteredItems;
        }
    }
}
