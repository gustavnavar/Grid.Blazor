using GridShared.Columns;

namespace GridShared.Filtering
{
    public interface IGridFilterSettings
    {
        IFilterColumnCollection FilteredColumns { get; }

        /// <summary>
        ///     Is filter settings int the init state
        /// </summary>
        bool IsInitState(IGridColumn column);
    }
}