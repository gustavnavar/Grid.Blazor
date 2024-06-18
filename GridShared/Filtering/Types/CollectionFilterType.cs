using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace GridShared.Filtering.Types
{
    /// <summary>
    ///     Object contains some logic for filtering Int32 columns
    /// </summary>
    public sealed class CollectionFilterType : FilterTypeBase
    {
        public override Type TargetType
        {
            get { return typeof (ICollection); }
        }

        public override GridFilterType GetValidType(GridFilterType type)
        {
            switch (type)
            {
                case GridFilterType.Equals:
                case GridFilterType.NotEquals:
                case GridFilterType.GreaterThan:
                case GridFilterType.LessThan:
                case GridFilterType.GreaterThanOrEquals:
                case GridFilterType.LessThanOrEquals:
                    return type;
                default:
                    return GridFilterType.Equals;
            }
        }

        public override object GetTypedValue(string value)
        {
            try
            {
                return int.Parse(value);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public override Expression GetFilterExpression<T>(Expression leftExpr, string value, GridFilterType filterType,
            Expression source, MethodInfo removeDiacritics)
        {
            filterType = GetValidType(filterType);

            object typedValue = GetTypedValue(value);
            if (typedValue == null && filterType != GridFilterType.IsDuplicated
                && filterType != GridFilterType.IsNotDuplicated)
                return null; //incorrent filter value;

            Expression valueExpr = Expression.Constant(typedValue);

            var pi = (PropertyInfo)((MemberExpression)leftExpr).Member;
            PropertyInfo count = pi.PropertyType.GetProperty("Count");
            leftExpr = Expression.Property(leftExpr, count);

            switch (filterType)
            {
                case GridFilterType.Equals:
                    return Expression.Equal(leftExpr, valueExpr);
                case GridFilterType.NotEquals:
                    return Expression.NotEqual(leftExpr, valueExpr);
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
    }
}