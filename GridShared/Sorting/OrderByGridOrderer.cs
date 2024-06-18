using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GridShared.Sorting
{
    /// <summary>
    ///     Object applies order (OrderBy, OrderByDescending) for items collection
    /// </summary>
    public class OrderByGridOrderer<T, TKey> : IColumnOrderer<T>
    {
        private readonly Expression<Func<T, TKey>> _expression;
        private readonly IComparer<TKey> _comparer;

        public OrderByGridOrderer(Expression<Func<T, TKey>> expression, IComparer<TKey> comparer)
        {
            _expression = expression;
            _comparer = comparer;
        }

        #region IColumnOrderer<T> Members

        public IQueryable<T> ApplyOrder(IQueryable<T> items, GridSortDirection direction)
        {
            if (typeof(TKey).IsGenericType && typeof(TKey).Name == "ICollection`1")
            {
                switch (direction)
                {
                    case GridSortDirection.Ascending:
                        return items.OrderBy(getCountExpresion());
                    case GridSortDirection.Descending:
                        return items.OrderByDescending(getCountExpresion());
                    default:
                        throw new ArgumentOutOfRangeException("direction");
                }
            }
            else
            {
                switch (direction)
                {
                    case GridSortDirection.Ascending:
                        if (_comparer == null)
                            return items.OrderBy(_expression);
                        else
                            return items.OrderBy(_expression, _comparer);
                    case GridSortDirection.Descending:
                        if (_comparer == null)
                            return items.OrderByDescending(_expression);
                        else
                            return items.OrderByDescending(_expression, _comparer);
                    default:
                        throw new ArgumentOutOfRangeException("direction");
                }
            }
        }

        public IQueryable<T> ApplyThenBy(IQueryable<T> items, GridSortDirection direction)
        {
            var ordered = items as IOrderedQueryable<T>;

            if (typeof(TKey).IsGenericType && typeof(TKey).Name == "ICollection`1")
            {
                switch (direction)
                {
                    case GridSortDirection.Ascending:
                        return ordered.ThenBy(getCountExpresion());
                    case GridSortDirection.Descending:
                        return ordered.ThenByDescending(getCountExpresion());
                    default:
                        throw new ArgumentOutOfRangeException("direction");
                }
            }
            else
            {
                switch (direction)
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
                        throw new ArgumentOutOfRangeException("direction");
                }
            }
        }

        public string GetOrderBy(GridSortDirection direction)
        {
            return GetBy(direction);
        }

        public string GetThenBy(GridSortDirection direction)
        {
            return GetBy(direction);
        }

        private string GetBy(GridSortDirection direction)
        {
            string result = "";

            // get column name
            List<string> names = new List<string>();
            Expression expression = _expression.Body;
            while (expression.NodeType != ExpressionType.Parameter)
            {
                names.Add(((MemberExpression)expression).Member.Name);
                expression = ((MemberExpression)expression).Expression;
            }
            for (int i = names.Count - 1; i >= 0; i--)
            {
                result += names[i];
                if (i != 0)
                    result += "/";
            }

            switch (direction)
            {
                case GridSortDirection.Ascending:
                    return result;
                case GridSortDirection.Descending:
                    return result + " desc";
                default:
                    throw new ArgumentOutOfRangeException("direction");
            }
        }


        private Expression<Func<T, Int32>> getCountExpresion()
        {
            ParameterExpression parameter = _expression.Parameters[0];

            var expression = (MemberExpression)_expression.Body;
            var pi = (PropertyInfo)expression.Member;

            PropertyInfo count = pi.PropertyType.GetProperty("Count");
            expression = Expression.Property(expression, count);
            return Expression.Lambda<Func<T, Int32>>(expression, parameter);
        }

        #endregion
    }
}