using GridShared.Columns;
using System.Collections.Generic;

namespace GridShared.Sorting
{
    /// <summary>
    ///     Collection of current order options for the grid
    /// </summary>
    public interface IOrderColumnCollection : IEnumerable<ColumnOrderValue>
    {
        /// <summary>
        ///     Get column order options by given grid column
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        IEnumerable<ColumnOrderValue> GetByColumn(IGridColumn column);
    }
}