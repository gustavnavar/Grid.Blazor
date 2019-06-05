namespace GridShared
{
    /// <summary>
    ///     Grid.Mvc interface
    /// </summary>
    public interface IGridOptions
    {
        /// <summary>
        ///    Grid items count
        /// </summary>
        int ItemsCount { get; }

        /// <summary>
        ///     Sum enabled for some columns
        /// </summary>
        bool IsSumEnabled { get; }

        /// <summary>
        ///     Average enabled for some columns
        /// </summary>
        bool IsAverageEnabled { get; }

        /// <summary>
        ///     Max enabled for some columns
        /// </summary>
        bool IsMaxEnabled { get; }

        /// <summary>
        ///     Min enabled for some columns
        /// </summary>
        bool IsMinEnabled { get; }
    }
}