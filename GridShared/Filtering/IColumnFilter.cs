using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GridShared.Filtering
{
    public interface IColumnFilter<T> : IColumnFilter
    {
        [Obsolete("This method is obsolete. Use the ApplyFilter method including an expresion parameter.", true)]
        IQueryable<T> ApplyFilter(IQueryable<T> items, IEnumerable<ColumnFilterValue> values, MethodInfo removeDiacritics = null);

        IQueryable<T> ApplyFilter(IQueryable<T> items, IEnumerable<ColumnFilterValue> values, Expression source, MethodInfo removeDiacritics = null);
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