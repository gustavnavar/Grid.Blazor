using System.Linq;

namespace GridShared.Sorting
{
    /// <summary>
    ///     Custom user column orderer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IColumnOrderer<T>
    {
        IQueryable<T> ApplyOrder(IQueryable<T> items, GridSortDirection direction);
        IQueryable<T> ApplyThenBy(IQueryable<T> items, GridSortDirection direction);
    }
}