using GridShared.Filtering;
using GridShared.Searching;
using GridShared.Sorting;

namespace GridCore
{
    /// <summary>
    ///     Setting for grid
    /// </summary>
    public interface IGridSettingsProvider
    {
        IGridSortSettings SortSettings { get; }
        IGridFilterSettings FilterSettings { get; }
        IGridSearchSettings SearchSettings { get; }
    }
}