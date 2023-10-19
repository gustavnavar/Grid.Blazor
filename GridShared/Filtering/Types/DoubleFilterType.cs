using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GridShared.Filtering.Types
{
    /// <summary>
    ///     Object contains some logic for filtering Double columns
    /// </summary>
    public sealed class DoubleFilterType : FilterTypeBase
    {
        public override Type TargetType
        {
            get { return typeof (Double); }
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
            double db;
            if (!double.TryParse(value, out db))
                return null;
            return db;
        }

        public override Expression GetFilterExpression<T>(Expression leftExpr, string value, GridFilterType filterType, 
            Expression source, MethodInfo removeDiacritics)
        {
            return GetFilterExpression<T, double>(leftExpr, value, filterType, source, removeDiacritics); ;
        }
    }
}