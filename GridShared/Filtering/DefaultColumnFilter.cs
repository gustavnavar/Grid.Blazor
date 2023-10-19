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
        private readonly PropertyInfo _pi;

        public bool IsNullable { get; private set; }

        public DefaultColumnFilter(Expression<Func<T, TData>> expression)
        {
            _expression = expression;
            _pi = (PropertyInfo)((MemberExpression)_expression.Body).Member;
            IsNullable = _pi.PropertyType.GetTypeInfo().IsGenericType &&
                              _pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        #region IColumnFilter<T> Members

        [Obsolete("This method is obsolete. Use the ApplyFilter method including an expresion parameter.", true)]
        public IQueryable<T> ApplyFilter(IQueryable<T> items, IEnumerable<ColumnFilterValue> values, MethodInfo removeDiacritics = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> ApplyFilter(IQueryable<T> items, IEnumerable<ColumnFilterValue> values,
            Expression source, MethodInfo removeDiacritics = null)
        {
            if (values == null && values.Where(r => r != ColumnFilterValue.Null).Count() <= 0)
                throw new ArgumentNullException("values");

            GridFilterCondition condition;
            var cond = values.SingleOrDefault(r => r != ColumnFilterValue.Null
                && r.FilterType == GridFilterType.Condition);
            if (!Enum.TryParse(cond.FilterValue, true, out condition))
                condition = GridFilterCondition.And;

            values = values.Where(r => r != ColumnFilterValue.Null && r.FilterType != GridFilterType.Condition);

            Expression<Func<T, bool>> expr = GetFilterExpression(values, condition, source, removeDiacritics);
            if (expr == null)
                return items;
            return items.Where(expr);
        }

        #endregion

        private Expression<Func<T, bool>> GetFilterExpression(IEnumerable<ColumnFilterValue> values,
            GridFilterCondition condition, Expression source, MethodInfo removeDiacritics)
        {
            Expression binaryExpression = null;
            foreach (var value in values)
            {
                if (value == ColumnFilterValue.Null)
                    continue;

                Expression expression = GetExpression(value, source, removeDiacritics);
                if (expression != null)
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

        private Expression GetExpression(ColumnFilterValue value, Expression source, MethodInfo removeDiacritics)
        {
            //get target type:
            Type targetType = IsNullable ? Nullable.GetUnderlyingType(_pi.PropertyType) : _pi.PropertyType;

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

                var nestedPi = (PropertyInfo)((MemberExpression)expression).Member;

                //detect nullable
                bool nestedIsNullable = nestedPi.PropertyType.GetTypeInfo().IsGenericType &&
                                  nestedPi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
                //get target type:
                Type nestedTargetType = nestedIsNullable ? Nullable.GetUnderlyingType(nestedPi.PropertyType) : nestedPi.PropertyType;

                // Check for null on nested properties and not value type (string and objects are reference type)
                // It's ok for ORM, but throw exception in linq to objects
                if (nestedIsNullable || !nestedTargetType.IsValueType)
                {
                    binaryExpression = binaryExpression == null ?
                        Expression.NotEqual(expression, Expression.Constant(null)) :
                        Expression.AndAlso(binaryExpression, Expression.NotEqual(expression, Expression.Constant(null)));
                }
            }

            // return expression for IsNull and IsNotNull
            if (value.FilterType == GridFilterType.IsNull)
            {
                value.FilterValue = "";
                if (targetType == typeof(string))
                    /* returns expression for IsNull, checking if the strings of a column
                     * are null or empty */
                    return GetExpression(null, value, targetType, source, removeDiacritics);
                else if (IsNullable)
                    return binaryExpression == null ?
                        Expression.Equal(_expression.Body, Expression.Constant(null)) :
                        Expression.OrElse(Expression.Not(binaryExpression), Expression.Equal(expression, Expression.Constant(null)));
                else
                    return binaryExpression == null ? null : Expression.Not(binaryExpression);
            }
            else if (value.FilterType == GridFilterType.IsNotNull)
            {
                value.FilterValue = "";
                if (targetType == typeof(string))
                    return GetExpression(binaryExpression, value, targetType, source, removeDiacritics);
                else if (IsNullable)
                    return binaryExpression == null ?
                        Expression.NotEqual(_expression.Body, Expression.Constant(null)) :
                        Expression.AndAlso(binaryExpression, Expression.NotEqual(expression, Expression.Constant(null)));
                else
                    return binaryExpression;
            }
            else
            {
                return GetExpression(binaryExpression, value, targetType, source, removeDiacritics);
            }
        }

        private Expression GetExpression(Expression binaryExpression, ColumnFilterValue value, Type targetType,
            Expression source, MethodInfo removeDiacritics)
        {
            IFilterType filterType = _typeResolver.GetFilterType(targetType);

            //support nullable types:
            Expression firstExpr = IsNullable
                                       ? Expression.Property(_expression.Body, _pi.PropertyType.GetProperty("Value"))
                                       : _expression.Body;

            var filterExpression = filterType.GetFilterExpression<T>(firstExpr, value.FilterValue, value.FilterType, source, 
                removeDiacritics);

            if (filterExpression == null) return null;

            if (IsNullable && targetType != typeof(string))
            {
                //add additional filter condition for check items on NULL with invoring "HasValue" method.
                //for example: result of this expression will like - c=> c.HasValue && c.Value = 3
                MemberExpression hasValueExpr = Expression.Property(_expression.Body,
                                                                    _pi.PropertyType.GetProperty("HasValue"));
                filterExpression = Expression.AndAlso(hasValueExpr, filterExpression);
            }

            binaryExpression = binaryExpression == null ? filterExpression :
                Expression.AndAlso(binaryExpression, filterExpression);

            //return filter expression
            return binaryExpression;
        }

        #region OData
        public string GetFilter(IEnumerable<ColumnFilterValue> values)
        {
            string result = "";

            if (values == null && values.Where(r => r != ColumnFilterValue.Null).Count() <= 0)
                throw new ArgumentNullException("values");

            // fet condition for multiple filters
            GridFilterCondition condition;
            var cond = values.SingleOrDefault(r => r != ColumnFilterValue.Null
                && r.FilterType == GridFilterType.Condition);
            if (!Enum.TryParse(cond.FilterValue, true, out condition))
                condition = GridFilterCondition.And;

            values = values.Where(r => r != ColumnFilterValue.Null && r.FilterType != GridFilterType.Condition);

            var pi = (PropertyInfo)((MemberExpression)_expression.Body).Member;
            foreach (var value in values)
            {
                if (value == ColumnFilterValue.Null)
                    continue;
                else if (value.FilterType != GridFilterType.IsNull && value.FilterType != GridFilterType.IsNotNull
                    && string.IsNullOrWhiteSpace(value.FilterValue))
                    continue;

                string filterString = GetFilterString(value);
                if (!string.IsNullOrWhiteSpace(filterString))
                {
                    if (string.IsNullOrWhiteSpace(result))
                        result = filterString;
                    else if (condition == GridFilterCondition.Or)
                        result += " or " + filterString;
                    else
                        result += " and " + filterString;
                }
            }
            return result;
        }

        private string GetFilterString(ColumnFilterValue value)
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

            if (!string.IsNullOrWhiteSpace(result))
            {
                if (value.FilterType == GridFilterType.IsNull)
                {
                    result += " eq null";
                }
                else if (value.FilterType == GridFilterType.IsNotNull)
                {
                    result += " ne null";
                }
                else
                {
                    //get target type:
                    Type targetType = IsNullable ? Nullable.GetUnderlyingType(_pi.PropertyType) : _pi.PropertyType;
                    
                    IFilterType filterType = _typeResolver.GetFilterType(targetType);
                    result = filterType.GetFilterExpression(result, value.FilterValue, value.FilterType);
                }
            }

            return result;
        }

        public bool IsTextColumn()
        {
            Type targetType = IsNullable ? Nullable.GetUnderlyingType(_pi.PropertyType) : _pi.PropertyType;
            IFilterType filterType = _typeResolver.GetFilterType(targetType);

            return filterType.GetType() == typeof(TextFilterType);
        }

        #endregion
    }
}