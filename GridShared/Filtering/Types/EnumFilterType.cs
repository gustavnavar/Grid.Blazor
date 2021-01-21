using System;

namespace GridShared.Filtering.Types
{
    /// <summary>
    ///     Object builds filter expressions for Enum grid columns
    /// </summary>
    public sealed class EnumFilterType : FilterTypeBase
    {
        public EnumFilterType(Type type) => TargetType = type;

        public override Type TargetType { get; }

        public override GridFilterType GetValidType(GridFilterType type)
            => GridFilterType.Equals;

        public override object GetTypedValue(string value)
            => Enum.IsDefined(TargetType, value) ? Enum.Parse(TargetType, value, true) : null;

        #region OData

        public override string GetFilterExpression(string columnName, string value, GridFilterType filterType)
        {
            value = GetStringValue(value).ToLower();

            //base implementation of building filter expressions
            filterType = GetValidType(filterType);

            switch (filterType)
            {
                case GridFilterType.Equals:
                    return columnName + " eq " + TargetType.FullName + "'" + value + "'";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}
