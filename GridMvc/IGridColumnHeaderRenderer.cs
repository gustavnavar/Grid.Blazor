using GridShared.Columns;
using Microsoft.AspNetCore.Html;

namespace GridMvc
{
    /// <summary>
    ///     Renderer of the header
    /// </summary>
    public interface IGridColumnHeaderRenderer
    {
        /// <summary>
        ///     Render grid header
        /// </summary>
        /// <param name="column">Column</param>
        /// <returns>HTML</returns>
        IHtmlContent Render(IGridColumn column);
    }
}