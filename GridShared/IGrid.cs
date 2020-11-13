using System;
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
        ///     Set or get default value of extended sorting
        /// </summary>
        bool ExtSortingEnabled { get; set; }

        /// <summary>
        ///     Set or get default value of grouping
        /// </summary>
        bool GroupingEnabled { get; set; }

        /// <summary>
        ///     Set or get visibility of ClearFiltersButton 
        /// </summary>
        bool ClearFiltersButtonEnabled { get; set; }

        /// <summary>
        ///     Set or get value of searching for all columns or only text ones
        /// </summary>
        bool SearchingOnlyTextColumns { get; set; }

        /// <summary>
        ///     Set or get value of searching for all columns including hidden ones
        /// </summary>
        bool SearchingHiddenColumns { get; set; }

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
        ///     Grid mode
        /// </summary>
        GridMode Mode { get; }

        /// <summary>
        ///     Get value for creating items
        /// </summary>
        bool CreateEnabled { get; }

        /// <summary>
        ///     Get value for reading items
        /// </summary>
        bool ReadEnabled { get; }

        /// <summary>
        ///     Get value for updating items
        /// </summary>
        bool UpdateEnabled { get; }

        /// <summary>
        ///     Get value for deleting items
        /// </summary>
        bool DeleteEnabled { get; }

        /// <summary>
        ///     Get value for table layout
        /// </summary>
        TableLayout TableLayout { get; }

        /// <summary>
        ///     Get value for table width
        /// </summary>
        string Width { get; }

        /// <summary>
        ///     Get value for table height
        /// </summary>
        string Height { get; }

        /// <summary>
        ///     Get all css classes mapped to the item
        /// </summary>
        string GetRowCssClasses(object item);

        /// <summary>
        ///     Get grid state
        /// </summary>
        string GetState();

        /// <summary>
        ///     Grid direction
        /// </summary>
        GridDirection Direction { get; set; }

        /// <summary>
        ///     Get column values to display
        /// </summary>
        IList<object> GetValuesToDisplay(string columnName, IEnumerable<object> items);

        /// <summary>
        ///     Grid items to display
        /// </summary>
        IEnumerable<object> GetItemsToDisplay(IList<Tuple<string, object>> values, IEnumerable<object> items);

        //void OnPreRender(); //TODO backward Compatibility
    }
}