using GridShared;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace GridMvc.Html
{
    /// <summary>
    ///     Grid adapter for html helper
    /// </summary>
    public class HtmlGrid<T> : GridHtmlOptions<T>, IGrid
    {
        public HtmlGrid(IHtmlHelper helper, SGrid<T> source, string viewName)
            : base(helper, source, viewName)
        {
        }

        IGridColumnCollection IGrid.Columns
        {
            get { return _source.Columns; }
        }

        public IEnumerable<object> ItemsToDisplay
        {
            get { return (_source as IGrid).ItemsToDisplay; }
        }

        //int IGrid.ItemsCount
        //{
        //    get { return _source.ItemsCount; }
        //    set { _source.ItemsCount = value; }
        //}

        public int DisplayingItemsCount
        {
            get { return _source.DisplayingItemsCount; }
        }

        public bool EnablePaging
        {
            get { return _source.EnablePaging; }
        }

        bool IGrid.SearchingEnabled {
            get { return _source.SearchingEnabled;  }
            set { _source.SearchingEnabled = value; }
        }

        public bool SearchingOnlyTextColumns {
            get { return _source.SearchingOnlyTextColumns; }
            set { _source.SearchingOnlyTextColumns = value; }
        }

        public bool SearchingHiddenColumns
        {
            get { return _source.SearchingHiddenColumns; }
            set { _source.SearchingHiddenColumns = value; }
        }

        public bool ExtSortingEnabled
        {
            get { return _source.ExtSortingEnabled; }
            set { _source.ExtSortingEnabled = value; }
        }

        public bool GroupingEnabled
        {
            get { return _source.GroupingEnabled; }
            set { _source.GroupingEnabled = value; }
        }

        public bool ClearFiltersButtonEnabled
        {
            get { return _source.ClearFiltersButtonEnabled; }
            set { _source.ClearFiltersButtonEnabled = value; }
        }

        public string EmptyGridText
        {
            get { return _source.EmptyGridText; }
        }

        public string Language
        {
            get { return _source.Language; }
        }

        public ISanitizer Sanitizer
        {
            get { return _source.Sanitizer; }
        }

        public GridMode Mode
        {
            get { return _source.Mode; }
        }

        public bool CreateEnabled
        {
            get { return _source.CreateEnabled; }
        }

        public bool ReadEnabled
        {
            get { return _source.ReadEnabled; }
        }

        public bool UpdateEnabled
        {
            get { return _source.UpdateEnabled; }
        }

        public bool DeleteEnabled
        {
            get { return _source.DeleteEnabled; }
        }

        public GridDirection Direction
        {
            get { return _source.Direction; }
            set { _source.Direction = value; }
        }

        public TableLayout TableLayout
        {
            get { return _source.TableLayout; }
        }

        public string Width
        {
            get { return _source.Width; }
        }

        public string Height
        {
            get { return _source.Height; }
        }

        string IGrid.GetRowCssClasses(object item)
        {
            return _source.GetRowCssClasses(item);
        }

        public string GetState()
        {
            return _source.GetState();
        }

        public IList<object> GetValuesToDisplay(string columnName, IEnumerable<object> items)
        {
            return _source.GetValuesToDisplay(columnName, items);
        }

        public IEnumerable<object> GetItemsToDisplay(IList<Tuple<string, object>> values, IEnumerable<object> items)
        {
            return _source.GetItemsToDisplay(values, items);
        }
    }
}