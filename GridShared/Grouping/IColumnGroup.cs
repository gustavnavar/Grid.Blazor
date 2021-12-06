using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GridShared.Grouping
{
    public interface IColumnGroup<T>
    {
        Func<object, Task<string>> GroupLabel { get; set; }
        
        Expression<Func<T, object>> GetColumnExpression();
        IQueryable<object> GetColumnValues(IQueryable<T> items);
        IQueryable<T> ApplyFilter(IQueryable<T> items, object value);
    }
}
