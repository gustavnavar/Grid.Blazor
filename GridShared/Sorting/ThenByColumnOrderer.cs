using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GridShared.Sorting
{
    /// <summary>
    ///     Object applies ThenBy and ThenByDescending order for items collection
    /// </summary>
    public class ThenByColumnOrderer<T, TKey> : IColumnOrderer<T>
    {
        private readonly Expression<Func<T, TKey>> _expression;
        private readonly IComparer<TKey> _comparer;
        private readonly GridSortDirection _initialDirection;

        public ThenByColumnOrderer(Expression<Func<T, TKey>> expression, IComparer<TKey> comparer, GridSortDirection initialDirection)
        {
            _expression = expression;
            _comparer = comparer;
            _initialDirection = initialDirection;
        }

        #region IColumnOrderer<T> Members

        private IQueryable<T> Apply(IQueryable<T> items)
        {
            var ordered = items as IOrderedQueryable<T>;
            if (ordered == null) return items; //not ordered collection
            switch (_initialDirection)
            {
                case GridSortDirection.Ascending:
                    if (_comparer == null)
                        return ordered.ThenBy(_expression);
                    else
                        return ordered.ThenBy(_expression, _comparer);
                case GridSortDirection.Descending:
                    if (_comparer == null)
                        return ordered.ThenByDescending(_expression);
                    else
                        return ordered.ThenByDescending(_expression, _comparer);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public IQueryable<T> ApplyOrder(IQueryable<T> items, GridSortDirection direction)
        {
            return Apply(items);
        }

        public IQueryable<T> ApplyThenBy(IQueryable<T> items, GridSortDirection direction)
        {
            return Apply(items);
        }
        
        #endregion
    }
}