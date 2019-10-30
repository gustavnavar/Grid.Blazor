using System;
using System.Linq;
using System.Linq.Expressions;

namespace GridShared.Sorting
{
    /// <summary>
    ///     Custom user column orderer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IColumnOrderer<T>
    {
        IQueryable<T> ApplyOrder(IQueryable<T> items);
        IQueryable<T> ApplyOrder(IQueryable<T> items, GridSortDirection direction);
        IQueryable<T> ApplyThenBy(IQueryable<T> items, GridSortDirection direction);
    }
}