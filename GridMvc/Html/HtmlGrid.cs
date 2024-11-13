using GridShared;
using GridShared.Grouping;
using GridShared.Pagination;
using GridShared.Style;
using GridShared.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GridMvc.Html
{
    /// <summary>
    ///     Grid adapter for html helper
    /// </summary>
    public class HtmlGrid<T> : GridHtmlOptions<T>, IGrid<T>
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

        public QueryDictionary<StringValues> Query
        {
            get { return _source.Query; }
            set { _source.Query = value; }
        }

        public int ItemsCount
        {
            get { return _source.ItemsCount; }
        }

        public int DisplayingItemsCount
        {
            get { return _source.DisplayingItemsCount; }
        }

        [Obsolete("This property is obsolete. Use PagingType property", true)]
        public bool EnablePaging
        {
            get
            {
                return _source.PagingType == PagingType.Pagination;
            }
            set { }
        }

        public PagingType PagingType
        {
            get { return _source.PagingType; }
            set { _source.PagingType = value; }
        }

        SearchOptions IGrid.SearchOptions {
            get { return _source.SearchOptions;  }
            set { _source.SearchOptions = value; }
        }

        public bool ExtSortingEnabled
        {
            get { return _source.ExtSortingEnabled; }
            set { _source.ExtSortingEnabled = value; }
        }

        public bool HiddenExtSortingHeader
        {
            get { return _source.HiddenExtSortingHeader; }
            set { _source.HiddenExtSortingHeader = value; }
        }

        public bool GroupingEnabled
        {
            get { return _source.GroupingEnabled; }
            set { _source.GroupingEnabled = value; }
        }

        public bool SyncButtonEnabled
        {
            get { return _source.SyncButtonEnabled; }
            set { _source.SyncButtonEnabled = value; }
        }

        public bool ClearFiltersButtonEnabled
        {
            get { return _source.ClearFiltersButtonEnabled; }
            set { _source.ClearFiltersButtonEnabled = value; }
        }

        public string EmptyGridText
        {
            get { return _source.EmptyGridText; }
            set { _source.EmptyGridText = value; }
        }

        public string Language
        {
            get { return _source.Language; }
            set { _source.Language = value; }
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
            set { _source.TableLayout = value; }
        }

        public string Width
        {
            get { return _source.Width; }
            set { _source.Width = value; }
        }

        public string Height
        {
            get { return _source.Height; }
            set { _source.Height = value; }
        }

        public MethodInfo RemoveDiacritics
        {
            get { return _source.RemoveDiacritics; }
            set { _source.RemoveDiacritics = value; }
        }

        public CssFramework CssFramework
        {
            get { return _source.CssFramework; }
            set { _source.CssFramework = value; }
        }

        public HtmlClass HtmlClass
        {
            get { return _source.HtmlClass; }
            set { _source.HtmlClass = value; }
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

        public IList<object> GetGroupValues(IColumnGroup<T> group, IEnumerable<object> items)
        {
            return _source.GetGroupValues(group, items);
        }

        public IColumnGroup<T> GetGroup(string columnName)
        {
            return _source.GetGroup(columnName);
        }

        public IEnumerable<object> GetItemsToDisplay(IList<Tuple<string, object>> values, IEnumerable<object> items)
        {
            return _source.GetItemsToDisplay(values, items);
        }
    }
}