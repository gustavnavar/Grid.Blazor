using GridShared.Sorting;
using GridShared.Utility;
using GridBlazor.Filtering;
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

        IQueryDictionary<StringValues> ToQuery();
    }
}