using GridShared.Columns;
using System.Collections.Generic;

namespace GridShared.Filtering
{
    /// <summary>
    ///     Collection of current filter options for the grid
    /// </summary>
    public interface IFilterColumnCollection : IEnumerable<ColumnFilterValue>
    {
        /// <summary>
        ///     Get column filter options by given grid column
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        IEnumerable<ColumnFilterValue> GetByColumn(IGridColumn column);
    }
}