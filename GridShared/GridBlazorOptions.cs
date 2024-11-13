using GridShared.Style;

namespace GridShared
{
    public class GridBlazorOptions : IGridBlazorOptions
    {

        /// <summary>
        ///     Default grid style
        /// </summary>
        public CssFramework Style { get; set; }
    }

    /// <summary>
    ///     GridBlazorOptions interface
    /// </summary>
    public interface IGridBlazorOptions
    {

        /// <summary>
        ///     Default grid style
        /// </summary>
        CssFramework Style { get; set; }
    }
}