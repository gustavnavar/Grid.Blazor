using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GridShared.Filtering.Types
{
    /// <summary>
    ///     Object contains some logic for filtering UInt16 columns
    /// </summary>
    public sealed class UInt16FilterType : FilterTypeBase
    {
        public override Type TargetType
        {
            get { return typeof(UInt16); }
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
                case GridFilterType.IsDuplicated:
                case GridFilterType.IsNotDuplicated:
                    return type;
                default:
                    return GridFilterType.Equals;
            }
        }

        public override object GetTypedValue(string value)
        {
            UInt16 i;
            if (!UInt16.TryParse(value, out i))
                return null;
            return i;
        }

        public override Expression GetFilterExpression<T>(Expression leftExpr, string value, GridFilterType filterType,
            Expression source, MethodInfo removeDiacritics)
        {
            return GetFilterExpression<T, UInt16>(leftExpr, value, filterType, source, removeDiacritics); ;
        }
    }
}
