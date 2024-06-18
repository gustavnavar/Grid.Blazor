using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

            if (typeof(TKey).IsGenericType && typeof(TKey).Name == "ICollection`1")
            {
                switch (_initialDirection)
                {
                    case GridSortDirection.Ascending:
                        return ordered.ThenBy(getCountExpresion());
                    case GridSortDirection.Descending:
                        return ordered.ThenByDescending(getCountExpresion());
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
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
        }

        public IQueryable<T> ApplyOrder(IQueryable<T> items, GridSortDirection direction)
        {
            return Apply(items);
        }

        public IQueryable<T> ApplyThenBy(IQueryable<T> items, GridSortDirection direction)
        {
            return Apply(items);
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

            switch (_initialDirection)
            {
                case GridSortDirection.Ascending:
                    return result;
                case GridSortDirection.Descending:
                    return result + " desc";
                default:
                    throw new ArgumentOutOfRangeException();
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