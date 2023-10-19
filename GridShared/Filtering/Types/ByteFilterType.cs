using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GridShared.Filtering.Types
{
    /// <summary>
    ///     Object contains some logic for filtering Byte columns
    /// </summary>
    public sealed class ByteFilterType : FilterTypeBase
    {
        /// <summary>
        ///     Get target filter type
        /// </summary>
        /// <returns></returns>
        public override Type TargetType
        {
            get { return typeof (Byte); }
        }

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
            byte bt;
            if (!byte.TryParse(value, out bt))
                return null;
            return bt;
        }

        public override Expression GetFilterExpression<T>(Expression leftExpr, string value, GridFilterType filterType, Expression source, MethodInfo removeDiacritics)
        {
            return GetFilterExpression<T, byte>(leftExpr, value, filterType, source, removeDiacritics); ;
        }
    }
}