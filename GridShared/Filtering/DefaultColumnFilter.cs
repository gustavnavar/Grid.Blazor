using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GridShared.Filtering.Types;

namespace GridShared.Filtering
{
    /// <summary>
    ///     Default Grid.Mvc filter. Provides logic for filtering items collection.
    /// </summary>
    public class DefaultColumnFilter<T, TData> : IColumnFilter<T>
    {
        private readonly Expression<Func<T, TData>> _expression;
        private readonly FilterTypeResolver _typeResolver = new FilterTypeResolver();

        public DefaultColumnFilter(Expression<Func<T, TData>> expression)
        {
            _expression = expression;
        }

        #region IColumnFilter<T> Members

        public IQueryable<T> ApplyFilter(IQueryable<T> items, IEnumerable<ColumnFilterValue> values)
        {
            if (values == null && values.Where(r => r != ColumnFilterValue.Null).Count() <= 0)
                throw new ArgumentNullException("values");

            var pi = (PropertyInfo) ((MemberExpression) _expression.Body).Member;

            GridFilterCondition condition;
            var cond = values.SingleOrDefault(r => r != ColumnFilterValue.Null
                && r.FilterType == GridFilterType.Condition);
            if (!Enum.TryParse(cond.FilterValue, true, out condition))
                condition = GridFilterCondition.And;

            values = values.Where(r => r != ColumnFilterValue.Null && r.FilterType != GridFilterType.Condition);
            
            Expression<Func<T, bool>> expr = GetFilterExpression(pi, values, condition);
            if (expr == null)
                return items;
            return items.Where(expr);
        }

        #endregion

        private Expression<Func<T, bool>> GetFilterExpression(PropertyInfo pi, IEnumerable<ColumnFilterValue> values,
            GridFilterCondition condition)
        {
            Expression binaryExpression = null;
            foreach (var value in values)
            {
                if (value == ColumnFilterValue.Null)
                    continue;

                Expression expression = GetExpression(pi, value);
                if(expression != null)
                {
                    if (binaryExpression == null)
                        binaryExpression = expression;
                    else if (condition == GridFilterCondition.Or)
                        binaryExpression = Expression.OrElse(binaryExpression, expression);
                    else
                        binaryExpression = Expression.AndAlso(binaryExpression, expression);
                }
            }

            if (binaryExpression == null)
                return null;

            //build expression to filter collection:
            ParameterExpression entityParam = _expression.Parameters[0];
            //return filter expression 
            return Expression.Lambda<Func<T, bool>>(binaryExpression, entityParam);
        }

        private Expression GetExpression(PropertyInfo pi, ColumnFilterValue value)
        {
            //detect nullable
            bool isNullable = pi.PropertyType.GetTypeInfo().IsGenericType &&
                              pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
            //get target type:
            Type targetType = isNullable ? Nullable.GetUnderlyingType(pi.PropertyType) : pi.PropertyType;

            List<string> names = new List<string>();
            Expression expression = _expression.Body;
            while (expression.NodeType != ExpressionType.Parameter)
            {
                names.Add(((MemberExpression)expression).Member.Name);
                expression = ((MemberExpression)expression).Expression;
            }

            Expression binaryExpression = null;
            for (int i = names.Count - 1; i >= 0; i--)
            {
                expression = Expression.Property(expression, names[i]);

                // Check for null on nested properties and target object if it's a string
                // It's ok for ORM, but throw exception in linq to objects
                if (i > 0 || (i == 0 && targetType == typeof(string)))
                {
                    binaryExpression = binaryExpression == null ?
                        Expression.NotEqual(expression, Expression.Constant(null)) :
                        Expression.AndAlso(binaryExpression, Expression.NotEqual(expression, Expression.Constant(null)));
                }
            }

            IFilterType filterType = _typeResolver.GetFilterType(targetType);
            
            //support nullable types:
            Expression firstExpr = isNullable
                                       ? Expression.Property(_expression.Body, pi.PropertyType.GetProperty("Value"))
                                       : _expression.Body;

            var filterExpression = filterType.GetFilterExpression(firstExpr, value.FilterValue, value.FilterType);

            if (filterExpression == null) return null;

            if (isNullable && targetType != typeof(string))
            {
                //add additional filter condition for check items on NULL with invoring "HasValue" method.
                //for example: result of this expression will like - c=> c.HasValue && c.Value = 3
                MemberExpression hasValueExpr = Expression.Property(_expression.Body,
                                                                    pi.PropertyType.GetProperty("HasValue"));
                filterExpression = Expression.AndAlso(hasValueExpr, filterExpression);
            }

            binaryExpression = binaryExpression == null ? filterExpression :
                Expression.AndAlso(binaryExpression, filterExpression);

            //return filter expression
            return binaryExpression;
        }
    }
}