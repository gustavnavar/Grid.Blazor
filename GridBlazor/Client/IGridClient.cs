using GridShared;
using System;
using System.Threading.Tasks;

namespace GridBlazor
{
    //// <summary>
    ///     Grid options for html helper
    /// </summary>
    public interface IGridClient<T>
    {
        IGridClient<T> Columns(Action<IGridColumnCollection<T>> columnBuilder);

        /// <summary>
        ///     Enable paging for grid
        /// </summary>
        /// <param name="pageSize">Setup the page size of the grid</param>
        IGridClient<T> WithPaging(int pageSize);

        /// <summary>
        ///     Enable paging for grid
        /// </summary>
        /// <param name="pageSize">Setup the page size of the grid</param>
        /// <param name="maxDisplayedItems">Setup max count of displaying pager links</param>
        IGridClient<T> WithPaging(int pageSize, int maxDisplayedItems);

        /// <summary>
        ///     Enable paging for grid
        /// </summary>
        /// <param name="pageSize">Setup the page size of the grid</param>
        /// <param name="maxDisplayedItems">Setup max count of displaying pager links</param>
        /// <param name="queryStringParameterName">Query string parameter name</param>
        IGridClient<T> WithPaging(int pageSize, int maxDisplayedItems, string queryStringParameterName);

        /// <summary>
        ///     Enable sorting for all columns
        /// </summary>
        IGridClient<T> Sortable();

        /// <summary>
        ///     Enable or disable sorting for all columns
        /// </summary>
        IGridClient<T> Sortable(bool enable);

        /// <summary>
        ///     Enable filtering for all columns
        /// </summary>
        IGridClient<T> Filterable();

        /// <summary>
        ///     Enable or disable filtering for all columns
        /// </summary>
        IGridClient<T> Filterable(bool enable);

        /// <summary>
        ///     Enable or disable client grid items selectable feature
        /// </summary>
        IGridClient<T> Selectable(bool set);

        /// <summary>
        ///     Setup the text, which will displayed with empty items collection in the grid
        /// </summary>
        /// <param name="text">Grid empty text</param>
        IGridClient<T> EmptyText(string text);

        /// <summary>
        ///     Setup the language of Grid.Mvc
        /// </summary>
        /// <param name="lang">SetLanguage string (example: "en", "ru", "fr" etc.)</param>
        IGridClient<T> SetLanguage(string lang);

        /// <summary>
        ///     Setup specific row css classes
        /// </summary>
        IGridClient<T> SetRowCssClasses(Func<T, string> contraint);

        /// <summary>
        ///     Specify Grid client name
        /// </summary>
        IGridClient<T> Named(string gridName);

        /// <summary>
        ///     Generates columns for all properties of the model.
        ///     Use data annotations to customize columns
        /// </summary>
        IGridClient<T> AutoGenerateColumns();

        /// <summary>
        ///     Allow grid to use multiple filters
        /// </summary>
        IGridClient<T> WithMultipleFilters();

        /// <summary>
        ///    Allow grid to show Grid items count
        /// </summary>
        IGridClient<T> WithGridItemsCount(string gridItemsName);

        /// <summary>
        ///    Allow grid to show Grid items count
        /// </summary>
        IGridClient<T> WithGridItemsCount();

        /// <summary>
        ///    Get grid object
        /// </summary>
        CGrid<T> Grid { get; }

        //void OnPreRender(); //TODO backward Compatibility

        /// <summary>
        ///    Set items from the server api
        /// </summary>
        Task UpdateGrid();
    }
}