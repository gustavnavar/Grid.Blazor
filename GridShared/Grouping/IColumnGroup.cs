using System;
using System.Linq;
using System.Linq.Expressions;

namespace GridShared.Grouping
{
    public interface IColumnGroup<T>
    {
        Expression<Func<T, object>> GetColumnExpression();
        IQueryable<object> GetColumnValues(IQueryable<T> items);
        IQueryable<T> ApplyFilter(IQueryable<T> items, object value);
    }
}
