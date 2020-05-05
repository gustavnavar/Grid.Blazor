using System.Collections.Generic;
using System.Linq;

namespace GridShared.Filtering
{
    public interface IColumnFilter<T> : IColumnFilter
    {
        IQueryable<T> ApplyFilter(IQueryable<T> items, IEnumerable<ColumnFilterValue> values);
    }

    public interface IColumnFilter
    {
        bool IsNullable { get; }

        // get OData string
        bool IsTextColumn();
        string GetFilter(IEnumerable<ColumnFilterValue> values);
    }
}