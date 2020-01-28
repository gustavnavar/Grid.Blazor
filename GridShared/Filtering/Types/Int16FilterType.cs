using System;

namespace GridShared.Filtering.Types
{
    /// <summary>
    ///     Object contains some logic for filtering Int32 columns
    /// </summary>
    public sealed class Int16FilterType : FilterTypeBase
    {
        public override Type TargetType
        {
            get { return typeof (Int16); }
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
            Int16 i;
            if (!Int16.TryParse(value, out i))
                return null;
            return i;
        }
    }
}