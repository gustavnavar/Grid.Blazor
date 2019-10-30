using System;
using System.Linq;
using System.Linq.Expressions;

namespace GridShared.Sorting
{
    /// <summary>
    ///     Object applies order (OrderBy, OrderByDescending) for items collection
    /// </summary>
    public class OrderByGridOrderer<T, TKey> : IColumnOrderer<T>
    {
        private readonly Expression<Func<T, TKey>> _expression;

        public OrderByGridOrderer(Expression<Func<T, TKey>> expression)
        {
            _expression = expression;
        }

        #region IColumnOrderer<T> Members

        public IQueryable<T> ApplyOrder(IQueryable<T> items)
        {
            return ApplyOrder(items, GridSortDirection.Ascending);
        }

        public IQueryable<T> ApplyOrder(IQueryable<T> items, GridSortDirection direction)
        {
            switch (direction)
            {
                case GridSortDirection.Ascending:
                    return items.OrderBy(_expression);
                case GridSortDirection.Descending:
                    return items.OrderByDescending(_expression);
                default:
                    throw new ArgumentOutOfRangeException("direction");
            }
        }

        public IQueryable<T> ApplyThenBy(IQueryable<T> items, GridSortDirection direction)
        {
            var ordered = items as IOrderedQueryable<T>;
            switch (direction)
            {
                case GridSortDirection.Ascending:
                    return ordered.ThenBy(_expression);
                case GridSortDirection.Descending:
                    return ordered.ThenByDescending(_expression);
                default:
                    throw new ArgumentOutOfRangeException("direction");
            }
        }

        #endregion
    }
}