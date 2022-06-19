using GridShared.Pagination;
using GridShared.Totals;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GridShared.Utility
{
    [DataContract]
    public class ItemsDTO<T>
    {
        [DataMember(Order = 1)]
        public IEnumerable<T> Items { get; set; }
        [DataMember(Order = 2)]
        public TotalsDTO Totals { get; set; }
        [DataMember(Order = 3)]
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
