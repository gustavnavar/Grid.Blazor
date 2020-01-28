using System;

namespace GridShared.Filtering.Types
{
    /// <summary>
    ///     Object contains some logic for filtering Single columns
    /// </summary>
    public sealed class SingleFilterType : FilterTypeBase
    {
        public override Type TargetType
        {
            get { return typeof (Single); }
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
            Single sng;
            if (!Single.TryParse(value, out sng))
                return null;
            return sng;
        }
    }
}