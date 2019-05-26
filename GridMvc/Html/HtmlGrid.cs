using GridShared;
using GridMvc.Pagination;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Collections.Generic;

namespace GridMvc.Html
{
    /// <summary>
    ///     Grid adapter for html helper
    /// </summary>
    public class HtmlGrid<T> : GridHtmlOptions<T>, ISGrid
    {
        private readonly SGrid<T> _source;


        public HtmlGrid(SGrid<T> source, ViewContext viewContext, string viewName, IViewEngine viewEngine)
            : base(source, viewContext, viewName, viewEngine)
        {
            _source = source;
        }

        public GridRenderOptions RenderOptions
        {
            get { return _source.RenderOptions; }
        }

        IGridColumnCollection IGrid.Columns
        {
            get { return _source.Columns; }
        }

        IEnumerable<object> IGrid.ItemsToDisplay
        {
            get { return (_source as IGrid).ItemsToDisplay; }
        }

        //int IGrid.ItemsCount
        //{
        //    get { return _source.ItemsCount; }
        //    set { _source.ItemsCount = value; }
        //}

        int IGrid.DisplayingItemsCount
        {
            get { return _source.DisplayingItemsCount; }
        }

        IGridPager ISGrid.Pager
        {
            get { return _source.Pager; }
        }

        bool IGrid.EnablePaging
        {
            get { return _source.EnablePaging; }
        }

        bool IGrid.SearchingEnabled {
            get { return _source.SearchingEnabled;  }
            set { _source.SearchingEnabled = value; }
        }

        bool IGrid.SearchingOnlyTextColumns {
            get { return _source.SearchingOnlyTextColumns; }
            set { _source.SearchingOnlyTextColumns = value; }
        }

        string IGrid.EmptyGridText
        {
            get { return _source.EmptyGridText; }
        }

        string IGrid.Language
        {
            get { return _source.Language; }
        }

        public ISanitizer Sanitizer
        {
            get { return _source.Sanitizer; }
        }

        string IGrid.GetRowCssClasses(object item)
        {
            return _source.GetRowCssClasses(item);
        }

        /// <summary>
        ///     To show Grid Items count
        ///     - Author by Jeeva
        /// </summary>
        int IGrid.ItemsCount
        {
            get { return _source.ItemsCount; }
        }

        IGridSettingsProvider ISGrid.Settings
        {
            get { return _source.Settings; }
        }
    }
}