using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GridShared.Filtering
{
    public interface IColumnFilter<T> : IColumnFilter
    {
        IQueryable<T> ApplyFilter(IQueryable<T> items, IEnumerable<ColumnFilterValue> values, MethodInfo removeDiacritics = null);
    }

    public interface IColumnFilter
    {
        bool IsNullable { get; }

        #region OData
        bool IsTextColumn();
        string GetFilter(IEnumerable<ColumnFilterValue> values);
        #endregion
    }
}