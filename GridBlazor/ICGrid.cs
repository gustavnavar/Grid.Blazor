using GridShared;
using GridBlazor.Pagination;
using System.Threading.Tasks;
using System;

namespace GridBlazor
{
    /// <summary>
    ///     Grid.Mvc interface
    /// </summary>
    public interface ICGrid : IGrid, IGridOptions
    {
        /// <summary>
        ///     Grid component options
        /// </summary>
        GridOptions ComponentOptions { get; }

        /// <summary>
        ///     Pager for the grid
        /// </summary>
        IGridPager Pager { get; }

        /// <summary>
        ///     Keys for subgrid
        /// </summary>
        string[] Keys { get; }

        /// <summary>
        ///     Subgrid clients
        /// </summary>
        Func<object[], Task<ICGrid>> SubGrids { get; }

        Type Type { get; }

        /// <summary>
        ///     Get foreign key values for subgrid records
        /// </summary>
        object[] GetKeyValues(object item);

        IGridSettingsProvider Settings { get; }

        /// <summary>
        ///    Set items from the server api
        /// </summary>
        Task UpdateGrid();
    }
}