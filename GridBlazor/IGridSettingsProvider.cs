using GridShared.Filtering;
using GridShared.Searching;
using GridShared.Sorting;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;

namespace GridBlazor
{
    /// <summary>
    ///     Setting for grid
    /// </summary>
    public interface IGridSettingsProvider
    {
        IGridSortSettings SortSettings { get; }
        IGridFilterSettings FilterSettings { get; }
        IGridSearchSettings SearchSettings { get; }

        IQueryDictionary<StringValues> ToQuery();
    }
}