using Microsoft.AspNetCore.Html;

namespace GridMvc
{
    /// <summary>
    ///     Renderer of the header
    /// </summary>
    public interface IGridSearchHeaderRenderer
    {
        /// <summary>
        ///     Render grid header
        /// </summary>
        /// <returns>HTML</returns>
        IHtmlContent Render();
    }
}