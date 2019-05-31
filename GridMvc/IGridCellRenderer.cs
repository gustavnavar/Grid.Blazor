using GridShared.Columns;
using Microsoft.AspNetCore.Html;

namespace GridMvc
{
    /// <summary>
    ///     Object to render the content
    /// </summary>
    public interface IGridCellRenderer
    {
        /// <summary>
        ///     Render grid cell
        /// </summary>
        /// <param name="column">Column of the cell</param>
        /// <param name="item">The row</param>
        /// <returns>HTML</returns>
        IHtmlContent Render(IGridColumn column, object item);
    }
}