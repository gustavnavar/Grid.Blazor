using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GridShared.Filtering.Types
{
    /// <summary>
    ///     Object builds filter expressions for boolean grid columns
    /// </summary>
    public sealed class BooleanFilterType : FilterTypeBase
    {
        /// <summary>
        ///     Get target filter type
        /// </summary>
        /// <returns></returns>
        public override Type TargetType
        {
            get { return typeof (Boolean); }
        }

        public override GridFilterType GetValidType(GridFilterType type)
        {
            //in any case Boolean types must compare by Equals filter type
            //We can't compare: contains(true) and etc.
            return GridFilterType.Equals;
        }

        public override object GetTypedValue(string value)
        {
            bool b;
            if (!bool.TryParse(value, out b))
                return null;
            return b;
        }

        public override Expression GetFilterExpression<T>(Expression leftExpr, string value, GridFilterType filterType, Expression source, MethodInfo removeDiacritics)
        {
            return GetFilterExpression<T, bool>(leftExpr, value, filterType, source, removeDiacritics); ;
        }
    }
}