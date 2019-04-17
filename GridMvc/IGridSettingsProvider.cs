using GridShared.Sorting;
using GridMvc.Filtering;

namespace GridMvc
{
    /// <summary>
    ///     Setting for grid
    /// </summary>
    public interface IGridSettingsProvider
    {
        IGridSortSettings SortSettings { get; }
        IGridFilterSettings FilterSettings { get; }
        IGridColumnHeaderRenderer GetHeaderRenderer();
    }
}