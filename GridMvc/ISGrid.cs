using GridShared;
using GridMvc.Html;
using GridMvc.Pagination;
using GridShared.Totals;

namespace GridMvc
{
    /// <summary>
    ///     Grid.Mvc interface
    /// </summary>
    public interface ISGrid : IGrid, IGridOptions
    {
        /// <summary>
        ///     Pager for the grid
        /// </summary>
        IGridPager Pager { get; }

        IGridSettingsProvider Settings { get; }

        GridRenderOptions RenderOptions { get; }

        TotalsDTO GetTotals();
    }
}