using System.Collections.Generic;
using System.Linq;

namespace GridShared.Filtering
{
    public interface IColumnFilter<T>
    {
        IQueryable<T> ApplyFilter(IQueryable<T> items, IEnumerable<ColumnFilterValue> value);
    }
}