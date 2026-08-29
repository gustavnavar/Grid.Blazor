using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace GridShared.Filtering.Types
{
    /// <summary>
    ///     Filtering for TimeSpan columns.
    ///     <para>
    ///     Without this the resolver fell through to <see cref="TextFilterType"/> - its own
    ///     comment calls that "not safe" - and a duration was compared as a string. The clock in
    ///     the filter would have looked right and asked the wrong question.
    ///     </para>
    ///     <para>
    ///     No conditional compilation here, unlike TimeOnly: <c>TimeSpan</c> exists in every
    ///     target framework this library builds for.
    ///     </para>
    /// </summary>
    public sealed class TimeSpanFilterType : FilterTypeBase
    {
        public override Type TargetType
        {
            get { return typeof(TimeSpan); }
        }

        public override Expression GetFilterExpression<T>(Expression leftExpr, string value, GridFilterType filterType,
            Expression source, MethodInfo removeDiacritics)
        {
            return GetFilterExpression<T, TimeSpan>(leftExpr, value, filterType, source, removeDiacritics);
        }

        /// <summary>
        ///     The comparisons a duration admits. The same set as a time of day: ordering makes
        ///     sense, and the text operators - contains, starts with - do not.
        /// </summary>
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

        /// <summary>
        ///     Reads the value the wire carries, which is <c>HH:mm</c> and invariant - never the
        ///     reader's culture. The picker has already converted by the time it gets here.
        /// </summary>
        public override object GetTypedValue(string value)
        {
            TimeSpan span;
            if (!TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out span))
                return null;
            return span;
        }
    }
}
