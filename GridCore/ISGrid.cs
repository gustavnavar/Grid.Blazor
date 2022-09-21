using GridCore.Filtering;
using GridCore.Pagination;
using GridCore.Searching;
using GridCore.Sorting;
using GridCore.Totals;
using GridShared;
using GridShared.Totals;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridCore
{
    public interface ISGrid<T> : ISGrid, IGrid<T>
    {
        new IGridColumnCollection<T> Columns { get; }

        IGridItemsProcessor<T> PagerProcessor { get; }
        IGridItemsProcessor<T> SearchProcessor { get; }
        IGridItemsProcessor<T> FilterProcessor { get; }
        IGridItemsProcessor<T> SortProcessor { get; }
        IGridItemsProcessor<T> TotalsProcessor { get; }

        void SetRowCssClassesContraint(Func<T, string> contraint);
        IEnumerable<T> GetItemsToDisplay();
        Task<IEnumerable<T>> GetItemsToDisplayAsync(Func<IQueryable<T>, Task<IList<T>>> toListAsync);

        void SetToListAsyncFunc(Func<IQueryable<T>, Task<IList<T>>> toListAsync);
    }

    public interface ISGrid : IGrid, IGridOptions
    {
        /// <summary>
        ///     Query for the grid
        /// </summary>
        QueryDictionary<StringValues> Query { get; }

        /// <summary>
        ///     Pager for the grid
        /// </summary>
        IGridPager Pager { get; set; }

        /// <summary>
        ///     Keys for subgrid
        /// </summary>
        string[] SubGridKeys { get; set; }

        /// <summary>
        ///     Get foreign key values for subgrid records
        /// </summary>
        string[] GetSubGridKeyValues(object item);

        IGridSettingsProvider Settings { get; }

        GridRenderOptions RenderOptions { get; }

        TotalsDTO GetTotals();

        bool DefaultSortEnabled { get; set; }

        bool DefaultFilteringEnabled { get; set; }

        void AutoGenerateColumns();

        Task<IEnumerable<object>> GetItemsToDisplayAsync();

        /// <summary>
        ///     Displaying grid items count
        /// </summary>
        Task<int> GetDisplayingItemsCountAsync();

        /// <summary>
        ///     Get column values to display
        /// </summary>
        IList<object> GetValuesToDisplay(string columnName, IEnumerable<object> items);
    }
}