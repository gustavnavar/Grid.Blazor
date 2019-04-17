using GridShared.Pagination;
using System.Collections.Generic;

namespace GridShared.Utility
{
    public class ItemsDTO<T>
    {
        public IEnumerable<T> Items { get; set; }
        public PagerDTO Pager { get; set; }

        public ItemsDTO()
        {
        }

        public ItemsDTO(IEnumerable<T> items, PagerDTO pager)
        {
            Items = items;
            Pager = pager;
        }
    }
}
