using GridShared.Filtering;
using GridShared.Searching;
using GridShared.Sorting;
using GridShared.Totals;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GridShared.Columns
{
    public interface IGridColumn<T> : IGridColumn, IColumn<T>, ISortableColumn<T>, IFilterableColumn<T>, ISearchableColumn<T>
    {      
    }

    public interface IGridColumn : ISortableColumn, IFilterableColumn
    {
        IGrid ParentGrid { get; }
        bool Hidden { get; }
    }

    /// <summary>
    ///     fluent interface for grid column
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IColumn<T>
    {
        /// <summary>
        ///     Set gridColumn title
        /// </summary>
        /// <param name="title">Title text</param>
        IGridColumn<T> Titled(string title);

        /// <summary>
        ///     Need to encode the content of the gridColumn
        /// </summary>
        /// <param name="encode">Yes/No</param>
        IGridColumn<T> Encoded(bool encode);

        /// <summary>
        ///     Sanitize column value from XSS attacks
        /// </summary>
        /// <param name="sanitize">If true values from this column will be sanitized</param>
        IGridColumn<T> Sanitized(bool sanitize);

        /// <summary>
        ///     Sets the width of the column
        /// </summary>
        IGridColumn<T> SetWidth(string width);

        /// <summary>
        ///     Sets the width of the column in pizels
        /// </summary>
        IGridColumn<T> SetWidth(int width);

        /// <summary>
        ///     Specify additional css class of the column
        /// </summary>
        IGridColumn<T> Css(string cssClasses);

        /// <summary>
        ///     Setup the custom classes for cells
        /// </summary>
        IGridColumn<T> SetCellCssClassesContraint(Func<T, string> contraint);

        /// <summary>
        ///     Setup the custom rendere for property
        /// </summary>
        IGridColumn<T> RenderValueAs(Func<T, string> constraint);

        /// <summary>
        ///     Setup the custom render for component
        /// </summary>
        IGridColumn<T> RenderComponentAs(Type componentType);

        /// <summary>
        ///     Format column values with specified text pattern
        /// </summary>
        IGridColumn<T> Format(string pattern);

        /// <summary>
        ///     Calculate Sum of column values
        /// </summary>
        IGridColumn<T> Sum(bool enabled);

        /// <summary>
        ///     Calculate average of column values
        /// </summary>
        IGridColumn<T> Average(bool enabled);

        /// <summary>
        ///     Calculate max of column values
        /// </summary>
        IGridColumn<T> Max(bool enabled);

        /// <summary>
        ///     Calculate min of column values
        /// </summary>
        IGridColumn<T> Min(bool enabled);
    }

    public interface IColumn
    {
        /// <summary>
        ///     Columns title
        /// </summary>
        string Title { get; }

        /// <summary>
        ///     Internal name of the gridColumn
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///     Width of the column
        /// </summary>
        string Width { get; set; }

        /// <summary>
        ///     EncodeEnabled
        /// </summary>
        bool EncodeEnabled { get; }

        bool SanitizeEnabled { get; }

        /// <summary>
        ///     Gets value of the gridColumn by instance
        /// </summary>
        /// <param name="instance">Instance of the item</param>
        IGridCell GetCell(object instance);

        /// <summary>
        ///     Get custom css classes mapped to the cell
        /// </summary>
        string GetCellCssClasses(object item);
    }

    /// <summary>
    ///     fluent interface for grid sorted column
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISortableColumn<T> : IColumn
    {
        /// <summary>
        ///     List of column orderes
        /// </summary>
        IEnumerable<IColumnOrderer<T>> Orderers { get; }

        /// <summary>
        ///     Enable sort of the gridColumn
        /// </summary>
        /// <param name="sort">Yes/No</param>
        IGridColumn<T> Sortable(bool sort);

        /// <summary>
        ///     Setup the initial sorting direction of current column
        /// </summary>
        /// <param name="direction">Ascending / Descending</param>
        IGridColumn<T> SortInitialDirection(GridSortDirection direction);

        /// <summary>
        ///     Setup ThenBy sorting of current column
        /// </summary>
        IGridColumn<T> ThenSortBy<TKey>(Expression<Func<T, TKey>> expression);

        /// <summary>
        ///     Setup ThenByDescending sorting of current column
        /// </summary>
        IGridColumn<T> ThenSortByDescending<TKey>(Expression<Func<T, TKey>> expression);
    }

    public interface ISortableColumn : IColumn
    {
        /// <summary>
        ///     Enable sort for this column
        /// </summary>
        bool SortEnabled { get; }

        /// <summary>
        ///     Is current column sorted
        /// </summary>
        bool IsSorted { get; set; }

        /// <summary>
        ///     Sort direction of current column
        /// </summary>
        GridSortDirection? Direction { get; set; }
    }

    public interface IFilterableColumn<T>
    {
        /// <summary>
        ///     Collection of current column filter
        /// </summary>
        IColumnFilter<T> Filter { get; }

        /// <summary>
        ///     Allows filtering for this column
        /// </summary>
        /// <param name="enalbe">Enable/disable filtering</param>
        IGridColumn<T> Filterable(bool enalbe);

        /// <summary>
        ///     Set up initial filter for this column
        /// </summary>
        /// <param name="type">Filter type</param>
        /// <param name="value">Filter value</param>
        IGridColumn<T> SetInitialFilter(GridFilterType type, string value);

        /// <summary>
        ///     Specify custom filter widget type for this column
        /// </summary>
        /// <param name="typeName">Widget type name</param>
        IGridColumn<T> SetFilterWidgetType(string typeName);

        /// <summary>
        ///     Specify custom filter widget type for this column
        /// </summary>
        /// <param name="typeName">Widget type name</param>
        /// <param name="widgetData">The data would be passed to the widget</param>
        IGridColumn<T> SetFilterWidgetType(string typeName, object widgetData);
    }

    public interface IFilterableColumn : IColumn
    {
        /// <summary>
        ///     Internal name of the gridColumn
        /// </summary>
        bool FilterEnabled { get; }

        /// <summary>
        ///     Initial filter settings for the column
        /// </summary>
        ColumnFilterValue InitialFilterSettings { get; set; }

        string FilterWidgetTypeName { get; }

        object FilterWidgetData { get; }
    }

    public interface ISearchableColumn<T>
    {
        /// <summary>
        ///     Collection of current column search
        /// </summary>
        IColumnSearch<T> Search { get; }
    }

    public interface ITotalsColumn<T>
    {
        /// <summary>
        ///     Collection of current column totals
        /// </summary>
        IColumnTotals<T> Totals { get; }
    }
}