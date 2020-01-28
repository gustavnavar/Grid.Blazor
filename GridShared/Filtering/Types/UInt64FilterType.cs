using System;

namespace GridShared.Filtering.Types
{
    /// <summary>
    ///     Object contains some logic for filtering UInt64 columns
    /// </summary>
    public sealed class UInt64FilterType : FilterTypeBase
    {
        public override Type TargetType
        {
            get { return typeof(UInt64); }
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
            UInt64 i;
            if (!UInt64.TryParse(value, out i))
                return null;
            return i;
        }
    }
}
