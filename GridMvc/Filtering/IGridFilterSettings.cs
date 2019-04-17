namespace GridMvc.Filtering
{
    public interface IGridFilterSettings
    {
        IFilterColumnCollection FilteredColumns { get; }

        /// <summary>
        ///     Is filter settings int the init state
        /// </summary>
        bool IsInitState { get; }
    }
}