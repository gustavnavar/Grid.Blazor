using System.Runtime.Serialization;

namespace GridShared.Sorting
{
    /// <summary>
    ///     Structure that specifies order settings for each column
    /// </summary>
    [DataContract]
    public struct ColumnOrderValue
    {
        public const string DefaultSortingQueryParameter = "grid-sorting";
        public const string SortingDataDelimeter = "__";

        [DataMember(Name = "ColumnName")]
        public string ColumnName { get; set; }

        [DataMember(Name = "Direction")] 
        public GridSortDirection Direction { get; set; }

        [DataMember(Name = "Id")]
        public int Id { get; set; }

        public ColumnOrderValue(string name, GridSortDirection direction, int id)
        {
            ColumnName = name;
            Direction = direction;
            Id = id;
        }

        public override string ToString()
        {
            return ColumnName + SortingDataDelimeter + Direction.ToString("D") + SortingDataDelimeter + Id.ToString();
        }

        public static ColumnOrderValue Null
        {
            get { return default(ColumnOrderValue); }
        }

        public static bool operator ==(ColumnOrderValue a, ColumnOrderValue b)
        {
            return a.ColumnName == b.ColumnName && a.Direction == b.Direction && a.Id == b.Id;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(ColumnOrderValue))
                return this == (ColumnOrderValue)obj;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return new { ColumnName, Direction, Id }.GetHashCode();
        }

        public static bool operator !=(ColumnOrderValue a, ColumnOrderValue b)
        {
            return a.ColumnName != b.ColumnName || a.Direction != b.Direction || a.Id == b.Id;
        }
    }
}