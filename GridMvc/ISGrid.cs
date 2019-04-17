using GridShared;
using GridMvc.Html;
using GridMvc.Pagination;

namespace GridMvc
{
    /// <summary>
    ///     Grid.Mvc interface
    /// </summary>
    public interface ISGrid : IGrid
    {
        /// <summary>
        ///     Pager for the grid
        /// </summary>
        IGridPager Pager { get; }

        IGridSettingsProvider Settings { get; }

        GridRenderOptions RenderOptions { get; }
    }
}