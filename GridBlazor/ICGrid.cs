using GridShared;
using GridBlazor.Pagination;
using System.Threading.Tasks;

namespace GridBlazor
{
    /// <summary>
    ///     Grid.Mvc interface
    /// </summary>
    public interface ICGrid<T> : IGrid
    {
        /// <summary>
        ///     Grid component options
        /// </summary>
        GridOptions ComponentOptions { get; }

        /// <summary>
        ///     Pager for the grid
        /// </summary>
        IGridPager Pager { get; }

        IGridSettingsProvider Settings { get; }

        /// <summary>
        ///    Set items from the server api
        /// </summary>
        Task UpdateGrid();
    }
}