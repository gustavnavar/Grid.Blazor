using GridShared.Columns;
using GridShared.Filtering;
using System.Collections.Generic;
using System.Linq;

namespace GridBlazor.Filtering
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

    public class DefaultFilterColumnCollection : List<ColumnFilterValue>, IFilterColumnCollection
    {
        public IEnumerable<ColumnFilterValue> GetByColumn(IGridColumn column)
        {
            return this.Where(c => c.ColumnName.ToUpper() == column.Name?.ToUpper());
        }
    }
}