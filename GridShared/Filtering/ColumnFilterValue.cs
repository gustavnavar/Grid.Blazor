using System.Net;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace GridShared.Filtering
{
    /// <summary>
    ///     Structure that specifies filter settings for each column
    /// </summary>
    [DataContract]
    public struct ColumnFilterValue
    {
        [DataMember(Name = "ColumnName")]
        public string ColumnName { get; set; }

        [DataMember(Name = "FilterType")] 
        public GridFilterType FilterType { get; set; }

        public string FilterValue;

        [DataMember(Name = "FilterValue")]
        [JsonPropertyName("FilterValue")]
        public string FilterValueEncoded
        {
            get { return WebUtility.UrlEncode(FilterValue); }
            set { FilterValue = value; }
        }

        public ColumnFilterValue(string name, GridFilterType type, string value)
        {
            ColumnName = name;
            FilterType = type;
            FilterValue = value;
        }

        public static ColumnFilterValue Null
        {
            get { return default(ColumnFilterValue); }
        }

        public static bool operator ==(ColumnFilterValue a, ColumnFilterValue b)
        {
            return a.ColumnName == b.ColumnName && a.FilterType == b.FilterType && a.FilterValue == b.FilterValue;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(ColumnFilterValue))
                return this == (ColumnFilterValue)obj;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return new { ColumnName, FilterType, FilterValue }.GetHashCode();
        }

        public static bool operator !=(ColumnFilterValue a, ColumnFilterValue b)
        {
            return a.ColumnName != b.ColumnName || a.FilterType != b.FilterType || a.FilterValue != b.FilterValue;
        }
    }
}