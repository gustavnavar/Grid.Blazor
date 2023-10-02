using GridShared;

namespace GridBlazor.Pagination
{
    public interface IGridPager
    {
        /// <summary>
        ///     Pager's Grid
        /// </summary>
        IGrid Grid { get; }

        /// <summary>
        ///     Max grid items, displaying on the page
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        ///     Enable changing page size on view
        /// </summary>
        bool ChangePageSize { get; set; }

        /// <summary>
        ///     Enable Go To field on view
        /// </summary>
        bool GoToVisibility { get; set; }

        /// <summary>
        ///     Max grid items, displaying on the page configured on the view
        /// </summary>
        int QueryPageSize { get; set; }

        /// <summary>
        ///     Current page index
        /// </summary>
        int CurrentPage { get; }

        int ItemsCount { get; set; }

        ///// <summary>
        /////     Total pages count
        ///// </summary>
        int PageCount { get; }

        /// <summary>
        ///     Start index
        /// </summary>
        int StartIndex { get; }

        /// <summary>
        ///     Virtualized items count
        /// </summary>
        int VirtualizedCount { get; }

        ///// <summary>
        /////     Starting displaying page
        ///// </summary>
        int StartDisplayedPage { get; }

        ///// <summary>
        /////     Last displaying page
        ///// </summary>
        int EndDisplayedPage { get; }

        //int MaxDisplayedPages { get; set; }

        //string ParameterName { get; }

        ///// <summary>
        /////     Получить адрес для конкретной страницы
        ///// </summary>
        ///// <param name="pageIndex">Номер страницы</param>
        ///// <returns>Адрес страницы</returns>
        //string GetLinkForPage(int pageIndex);

        /// <summary>
        ///     Label to show which page the user currently is on
        /// </summary>
        string CurrentPageLabel { get; set; }
    }
}