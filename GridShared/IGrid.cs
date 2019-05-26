using System.Collections.Generic;

namespace GridShared
{
    /// <summary>
    ///     Grid.Mvc interface
    /// </summary>
    public interface IGrid
    {
        /// <summary>
        ///     Grid columns
        /// </summary>
        IGridColumnCollection Columns { get; }

        /// <summary>
        ///     Grid items
        /// </summary>
        IEnumerable<object> ItemsToDisplay { get; }

        /// <summary>
        ///     Displaying grid items count
        /// </summary>
        int DisplayingItemsCount { get; }

        /// <summary>
        ///     Enable paging view
        /// </summary>
        bool EnablePaging { get; }

        /// <summary>
        ///     Set or get default value of searching
        /// </summary>
        bool SearchingEnabled { get; set; }

        /// <summary>
        ///     Set or get value of searching for all columns or only text ones
        /// </summary>
        bool SearchingOnlyTextColumns { get; set; }

        /// <summary>
        ///     Text in empty grid (no items for display)
        /// </summary>
        string EmptyGridText { get; }

        /// <summary>
        ///     Returns the current Grid language
        /// </summary>
        string Language { get; }

        /// <summary>
        ///     Object that sanitize grid column values from dangerous content
        /// </summary>
        ISanitizer Sanitizer { get; }

        /// <summary>
        ///     Get all css classes mapped to the item
        /// </summary>
        string GetRowCssClasses(object item);

        //void OnPreRender(); //TODO backward Compatibility

        int ItemsCount { get; }
    }
}