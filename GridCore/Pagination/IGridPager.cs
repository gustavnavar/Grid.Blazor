namespace GridCore.Pagination
{
    public interface IGridPager
    {
        /// <summary>
        ///     Partial view name to render the pager
        /// </summary>
        string TemplateName { get; }

        /// <summary>
        ///     Method invokes before pager render
        /// </summary>
        void Initialize(int count);

        /// <summary>
        ///     Max grid items, displaying on the page
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        ///     Enable changing page size on view
        /// </summary>
        bool ChangePageSize { get; set; }

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
        //int PageCount { get; }

        ///// <summary>
        /////     Starting displaying page
        ///// </summary>
        //int StartDisplayedPage { get; }

        ///// <summary>
        /////     Last displaying page
        ///// </summary>
        //int EndDisplayedPage { get; }

        //int MaxDisplayedPages { get; set; }

        //string ParameterName { get; }

        //int ItemsCount { get; set; }

        ///// <summary>
        /////     Получить адрес для конкретной страницы
        ///// </summary>
        ///// <param name="pageIndex">Номер страницы</param>
        ///// <returns>Адрес страницы</returns>
        //string GetLinkForPage(int pageIndex);
    }
}