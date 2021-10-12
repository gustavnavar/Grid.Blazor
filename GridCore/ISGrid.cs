using GridCore.Pagination;
using GridShared;
using GridShared.Totals;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace GridCore
{
    public interface ISGrid<T> : ISGrid
    {
        new IGridColumnCollection<T> Columns { get; }

        void SetRowCssClassesContraint(Func<T, string> contraint);
        IEnumerable<T> GetItemsToDisplay();
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
    }
}