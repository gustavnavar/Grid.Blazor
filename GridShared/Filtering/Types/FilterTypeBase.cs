using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GridShared.Filtering.Types
{
    /// <summary>
    ///     Base filter type for all filter types
    /// </summary>
    public abstract class FilterTypeBase : IFilterType
    {
        #region IFilterType Members

        /// <summary>
        ///     Sanitize filter type for specific column data type
        /// </summary>
        /// <param name="type">Filter type (equals, contains etc)</param>
        /// <returns>Sanitized filter type</returns>
        public abstract GridFilterType GetValidType(GridFilterType type);

        /// <summary>
        ///     Return typed object from text representation (query string parameter value)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract object GetTypedValue(string value);

        /// <summary>
        ///     Get target filter type
        /// </summary>
        /// <returns></returns>
        public abstract Type TargetType { get; }

        public abstract Expression GetFilterExpression<T>(Expression leftExpr, string value, GridFilterType filterType,
            Expression source, MethodInfo removeDiacritics);

        [Obsolete("This method is obsolete. Use the GetFilterExpression<T> method including an expresion parameter.", true)]
        public Expression GetFilterExpression(Expression leftExpr, string value, GridFilterType filterType,
           MethodInfo removeDiacritics = null)
        { 
            throw new NotImplementedException();
        }

        protected Expression GetFilterExpression<T, S>(Expression leftExpr, string value, GridFilterType filterType,
            Expression source, MethodInfo removeDiacritics)
        {
            //base implementation of building filter expressions
            filterType = GetValidType(filterType);

            object typedValue = GetTypedValue(value);
            if (typedValue == null)
                return null; //incorrent filter value;

            Type targetType = TargetType;

            Expression valueExpr = Expression.Constant(typedValue);

            switch (filterType)
            {
                case GridFilterType.Equals:
                    return Expression.Equal(leftExpr, valueExpr);
                case GridFilterType.NotEquals:
                    return Expression.NotEqual(leftExpr, valueExpr);
                case GridFilterType.Contains:
                    MethodInfo miContains = TargetType.GetMethod("Contains", new[] {typeof (string)});
                    return Expression.Call(leftExpr, miContains, valueExpr);
                case GridFilterType.StartsWith:
                    MethodInfo miStartsWith = targetType.GetMethod("StartsWith", new[] {typeof (string)});
                    return Expression.Call(leftExpr, miStartsWith, valueExpr);
                case GridFilterType.EndsWidth:
                    MethodInfo miEndssWith = targetType.GetMethod("EndsWith", new[] {typeof (string)});
                    return Expression.Call(leftExpr, miEndssWith, valueExpr);
                case GridFilterType.LessThan:
                    return Expression.LessThan(leftExpr, valueExpr);
                case GridFilterType.LessThanOrEquals:
                    return Expression.LessThanOrEqual(leftExpr, valueExpr);
                case GridFilterType.GreaterThan:
                    return Expression.GreaterThan(leftExpr, valueExpr);
                case GridFilterType.GreaterThanOrEquals:
                    return Expression.GreaterThanOrEqual(leftExpr, valueExpr);
                case GridFilterType.IsDuplicated:
                    Expression groupBy = GetGroupBy<T, S>(source, leftExpr);
                    MethodInfo methodInfo = typeof(Queryable).GetMethods()
                        .Single(r => r.Name == "Contains" && r.GetParameters().Length == 2)
                        .MakeGenericMethod(new Type[] { typeof(S) });
                    return Expression.Call(groupBy, methodInfo, leftExpr);
                case GridFilterType.IsNotDuplicated:
                    groupBy = GetGroupBy<T, S>(source, leftExpr);
                    methodInfo = typeof(Queryable).GetMethods()
                        .Single(r => r.Name == "Contains" && r.GetParameters().Length == 2)
                        .MakeGenericMethod(new Type[] { typeof(S) });
                    var expresion = Expression.Call(groupBy, methodInfo, leftExpr);
                    return Expression.Not(expresion);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        protected Expression GetGroupBy<T, S>(Expression source, Expression expression)
        {
            if (expression == null)
                return null;

            List<string> names = new List<string>();
            while (expression.NodeType != ExpressionType.Parameter)
            {
                names.Add(((MemberExpression)expression).Member.Name);
                expression = ((MemberExpression)expression).Expression;
            }

            Type entityType =  typeof(T);
            ParameterExpression parameter = Expression.Parameter(entityType, "c");

            Expression binaryExpression = parameter;
            for (int i = names.Count - 1; i >= 0; i--)
            {
                binaryExpression = Expression.Property(binaryExpression, names[i]);
            }

            var selector = Expression.Lambda<Func<T, S>>(binaryExpression, parameter);

            var methodInfo = typeof(Queryable).GetMethods()
                .Single(r => r.Name == "GroupBy" && r.GetParameters().Length == 2)
                .MakeGenericMethod(new Type[] { typeof(T), typeof(S) });
            binaryExpression = Expression.Call(methodInfo, source, selector);

            methodInfo = typeof(Queryable).GetMethods()
                .Where(r => r.Name == "Where" && r.GetParameters().Length == 2)
                .First()
                .MakeGenericMethod(new Type[] { typeof(IGrouping<S, T>) }); ;
            Expression<Func<IGrouping<S, T>, bool>> having = c => c.Count() > 1;
            binaryExpression = Expression.Call(methodInfo, binaryExpression, having);

            methodInfo = typeof(Queryable).GetMethods()
                .Where(r => r.Name == "Select" && r.GetParameters().Length == 2)
                .First()
                .MakeGenericMethod(new Type[] { typeof(IGrouping<S, T>), typeof(S) });
            Expression<Func<IGrouping<S, T>, S>> select = c => c.Key;
            binaryExpression = Expression.Call(methodInfo, binaryExpression, select);

            return binaryExpression;
        }

        #region OData

        public virtual string GetFilterExpression(string columnName, string value, GridFilterType filterType)
        {
            value = GetStringValue(value);

            //base implementation of building filter expressions
            filterType = GetValidType(filterType);        

            switch (filterType)
            {
                case GridFilterType.Equals:
                    return columnName + " eq " + value;
                case GridFilterType.NotEquals:
                    return columnName + " ne " + value;
                case GridFilterType.Contains:
                    return "contains(" + columnName + ", " + value + ")";
                case GridFilterType.StartsWith:
                    return "startswith(" + columnName + ", " + value + ")";
                case GridFilterType.EndsWidth:
                    return "endswith(" + columnName + ", " + value + ")";
                case GridFilterType.LessThan:
                    return columnName + " lt " + value;
                case GridFilterType.LessThanOrEquals:
                    return columnName + " le " + value;
                case GridFilterType.GreaterThan:
                    return columnName + " gt " + value;
                case GridFilterType.GreaterThanOrEquals:
                    return columnName + " ge " + value;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public virtual string GetStringValue(string value)
        {
            return value;
        }

        #endregion
    }
}