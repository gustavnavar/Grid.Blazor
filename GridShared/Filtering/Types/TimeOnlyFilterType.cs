#if ! NETSTANDARD2_1 && !NET5_0
using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace GridShared.Filtering.Types
{
    /// <summary>
    ///     Object contains some logic for filtering TimeOnly columns
    /// </summary>
    public sealed class TimeOnlyFilterType : FilterTypeBase
    {
        public override Type TargetType
        {
            get { return typeof(TimeOnly); }
        }

        public override Expression GetFilterExpression<T>(Expression leftExpr, string value, GridFilterType filterType,
            Expression source, MethodInfo removeDiacritics)
        {
            //var dateExpr = Expression.Property(leftExpr, leftExpr.Type, "Date");

            //if (filterType == GridFilterType.Equals)
            //{
            //    var dateObj = GetTypedValue(value);
            //    if (dateObj == null) return null;//not valid

            //    var startDate = Expression.Constant(dateObj);
            //    var endDate = Expression.Constant(((TimeOnly)dateObj).AddDays(1));

            //    var left = Expression.GreaterThanOrEqual(leftExpr, startDate);
            //    var right = Expression.LessThan(leftExpr, endDate);

            //    return Expression.And(left, right);
            //}

            return GetFilterExpression<T, TimeOnly>(leftExpr, value, filterType, source, removeDiacritics);
        }

        /// <summary>
        ///     There are filter types that allowed for TimeOnly column
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override GridFilterType GetValidType(GridFilterType type)
        {
            switch (type)
            {
                case GridFilterType.Equals:
                case GridFilterType.NotEquals:
                case GridFilterType.GreaterThan:
                case GridFilterType.GreaterThanOrEquals:
                case GridFilterType.LessThan:
                case GridFilterType.LessThanOrEquals:
                case GridFilterType.IsDuplicated:
                case GridFilterType.IsNotDuplicated:
                    return type;
                default:
                    return GridFilterType.Equals;
            }
        }

        public override object GetTypedValue(string value)
        {
            TimeOnly date;
            if (!TimeOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                return null;
            return date;
        }
    }
}
#endif