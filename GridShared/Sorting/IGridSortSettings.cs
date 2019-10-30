using GridShared.Utility;
using Microsoft.Extensions.Primitives;

namespace GridShared.Sorting
{
    /// <summary>
    ///     SortSettings for sort
    /// </summary>
    public interface IGridSortSettings
    {
        IQueryDictionary<StringValues> Query { get; }

        /// <summary>
        ///     Column name for sort
        /// </summary>
        string ColumnName { get; set; }

        /// <summary>
        ///     Direction of sorting
        /// </summary>
        GridSortDirection Direction { get; set; }

        DefaultOrderColumnCollection SortValues { get;  }
    }
}