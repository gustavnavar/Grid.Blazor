using GridMvc.Html;
using GridMvc.Pagination;
using GridShared;
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

        /// <summary>
        ///     Keys for subgrid
        /// </summary>
        string[] SubGridKeys { get; }

        /// <summary>
        ///     Get foreign key values for subgrid records
        /// </summary>
        string[] GetSubGridKeyValues(object item);

        IGridSettingsProvider Settings { get; }

        GridRenderOptions RenderOptions { get; }

        TotalsDTO GetTotals();
    }
}