using System;

namespace GridShared.Filtering.Types
{
    /// <summary>
    ///     Object contains some logic for filtering decimal columns
    /// </summary>
    public sealed class DecimalFilterType : FilterTypeBase
    {
        public override Type TargetType
        {
            get { return typeof (Decimal); }
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
            decimal dec;
            if (!decimal.TryParse(value, out dec))
                return null;
            return dec;
        }
    }
}