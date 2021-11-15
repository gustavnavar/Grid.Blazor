using System;
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

        public virtual Expression GetFilterExpression(Expression leftExpr, string value, GridFilterType filterType,
            MethodInfo removeDiacritics)
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

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