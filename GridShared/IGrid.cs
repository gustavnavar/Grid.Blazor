﻿using GridShared.Grouping;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GridShared
{
    public interface IGrid<T> : IGrid
    {
        IColumnGroup<T> GetGroup(string columnName);
        IList<object> GetGroupValues(IColumnGroup<T> group, IEnumerable<object> items);
    }

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
        bool EnablePaging { get; set; }

        /// <summary>
        ///     Set or get default value of searching
        /// </summary>
        bool SearchingEnabled { get; set; }

        /// <summary>
        ///     Set or get default value of extended sorting
        /// </summary>
        bool ExtSortingEnabled { get; set; }

        /// <summary>
        ///     Hide extended sorting / grouping header
        /// </summary>
        bool HiddenExtSortingHeader { get; set; }

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
        string EmptyGridText { get; set; }

        /// <summary>
        ///     Returns the current Grid language
        /// </summary>
        string Language { get; set; }

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
        TableLayout TableLayout { get; set; }

        /// <summary>
        ///     Get value for table width
        /// </summary>
        string Width { get; set; }

        /// <summary>
        ///     Get value for table height
        /// </summary>
        string Height { get; set; }

        /// <summary>
        ///     Database function to remove diacritics
        /// </summary>
        MethodInfo RemoveDiacritics { get; set; }

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
        ///     Grid items to display
        /// </summary>
        IEnumerable<object> GetItemsToDisplay(IList<Tuple<string, object>> values, IEnumerable<object> items);

        //void OnPreRender(); //TODO backward Compatibility
    }
}