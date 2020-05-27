﻿using GridMvc.Pagination;
using GridMvc.Resources;
using GridShared;
using GridShared.Columns;
using GridShared.Pagination;
using GridShared.Utility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace GridMvc.Server

{
    /// <summary>
    ///     Grid adapter for html helper
    /// </summary>
    public class GridServer<T> : IGridServer<T>
    {
        private readonly SGrid<T> _source;

        public GridServer(IEnumerable<T> items, IQueryCollection query, bool renderOnlyRows, 
            string gridName, Action<IGridColumnCollection<T>> columns = null, int? pageSize = null, 
            string language = "", string pagerViewName = GridPager.DefaultPagerViewName,
            IColumnBuilder<T> columnBuilder = null)
        {
            _source = new SGrid<T>(items, query, renderOnlyRows, pagerViewName, columnBuilder);
            _source.RenderOptions.GridName = gridName;
            columns?.Invoke(_source.Columns);
            if (!string.IsNullOrWhiteSpace(language))
                _source.Language = language;
            if(pageSize.HasValue)
                WithPaging(pageSize.Value);
        }

        #region IGridHtmlOptions<T> Members

        public IGridServer<T> WithGridItemsCount()
        {
            return WithGridItemsCount(string.Empty);
        }

        public IGridServer<T> Columns(Action<IGridColumnCollection<T>> columnBuilder)
        {
            columnBuilder(_source.Columns);
            return this;
        }

        public IGridServer<T> ChangePageSize(bool enable)
        {
            _source.Pager.ChangePageSize = enable;
            return this;
        }

        public IGridServer<T> WithPaging(int pageSize)
        {
            return WithPaging(pageSize, 0);
        }

        public IGridServer<T> WithPaging(int pageSize, int maxDisplayedItems)
        {
            return WithPaging(pageSize, maxDisplayedItems, string.Empty);
        }

        public IGridServer<T> WithPaging(int pageSize, int maxDisplayedItems, string queryStringParameterName)
        {
            _source.EnablePaging = true;
            _source.Pager.PageSize = pageSize;

            var pager = _source.Pager as GridPager; //This setting can be applied only to default grid pager
            if (pager == null) return this;

            if (maxDisplayedItems > 0)
                pager.MaxDisplayedPages = maxDisplayedItems;
            if (!string.IsNullOrEmpty(queryStringParameterName))
                pager.ParameterName = queryStringParameterName;
            _source.Pager = pager;
            return this;
        }

        public IGridServer<T> Sortable()
        {
            return Sortable(true);
        }

        public IGridServer<T> Sortable(bool enable)
        {
            _source.DefaultSortEnabled = enable;
            foreach (IGridColumn column in _source.Columns)
            {
                var typedColumn = column as IGridColumn<T>;
                if (typedColumn == null) continue;
                typedColumn.Sortable(enable);
            }
            return this;
        }

        public IGridServer<T> Filterable()
        {
            return Filterable(true);
        }

        public IGridServer<T> Filterable(bool enable)
        {
            _source.DefaultFilteringEnabled = enable;
            foreach (IGridColumn column in _source.Columns)
            {
                var typedColumn = column as IGridColumn<T>;
                if (typedColumn == null) continue;
                typedColumn.Filterable(enable);
            }
            return this;
        }

        public IGridServer<T> Searchable()
        {
            return Searchable(true, true);
        }

        public IGridServer<T> Searchable(bool enable)
        {
            return Searchable(enable, true);
        }

        public IGridServer<T> Searchable(bool enable, bool onlyTextColumns)
        {
            return Searchable(enable, onlyTextColumns, false);
        }

        public IGridServer<T> Searchable(bool enable, bool onlyTextColumns, bool hiddenColumns)
        {
            _source.SearchingEnabled = enable;
            _source.SearchingOnlyTextColumns = onlyTextColumns;
            _source.SearchingHiddenColumns = hiddenColumns;
            return this;
        }

        public IGridServer<T> ExtSortable()
        {
            return ExtSortable(true);
        }

        public IGridServer<T> ExtSortable(bool enable)
        {
            _source.ExtSortingEnabled = enable;
            return this;
        }      

        public IGridServer<T> Groupable()
        {
            return Groupable(true);
        }

        public IGridServer<T> Groupable(bool enable)
        {
            _source.ExtSortingEnabled = enable;
            _source.GroupingEnabled = enable;
            return this;
        }

        public IGridServer<T> ClearFiltersButton(bool enable)
        {
            _source.ClearFiltersButtonEnabled = enable;
            return this;
        }

        public IGridServer<T> Selectable(bool set)
        {
            _source.RenderOptions.Selectable = set;
            return this;
        }

        public IGridServer<T> EmptyText(string text)
        {
            _source.EmptyGridText = text;
            return this;
        }

        public IGridServer<T> SetLanguage(string lang)
        {
            _source.Language = lang;
            return this;
        }

        public IGridServer<T> SetRowCssClasses(Func<T, string> contraint)
        {
            _source.SetRowCssClassesContraint(contraint);
            return this;
        }

        public IGridServer<T> Named(string gridName)
        {
            _source.RenderOptions.GridName = gridName;
            return this;
        }

        /// <summary>
        ///     Generates columns for all properties of the model.
        ///     Use data annotations to customize columns
        /// </summary>
        public IGridServer<T> AutoGenerateColumns()
        {
            _source.AutoGenerateColumns();
            return this;
        }

        public IGridServer<T> WithMultipleFilters()
        {
            _source.RenderOptions.AllowMultipleFilters = true;
            return this;
        }

        /// <summary>
        ///     Set to true if we want to show grid itmes count
        ///     - Author - Jeeva J
        /// </summary>
        public IGridServer<T> WithGridItemsCount(string gridItemsName)
        {
            if (string.IsNullOrWhiteSpace(gridItemsName))
                gridItemsName = Strings.Items;

            _source.RenderOptions.GridCountDisplayName = gridItemsName;
            _source.RenderOptions.ShowGridItemsCount = true;
            return this;
        }

        /// <summary>
        ///     Enable or disable striped grid
        /// </summary>
        public IGridServer<T> SetStriped(bool enable)
        {
            _source.RenderOptions.Striped = enable;
            return this;
        }

        /// <summary>
        ///    Allow grid to show a SubGrid
        /// </summary>
        public IGridServer<T> SubGrid(params string[] keys)
        {
            _source.SubGridKeys = keys;
            return this;
        }

        /// <summary>
        ///     Items, displaying in the grid view
        /// </summary>
        public ItemsDTO<T> ItemsToDisplay
        {
            get {
                var items = _source.GetItemsToDisplay();
                var totals = _source.GetTotals();
                return new ItemsDTO<T>(items, totals, new PagerDTO(_source.EnablePaging, _source.Pager.PageSize,
                    _source.Pager.CurrentPage, _source.ItemsCount));
            }
        }

        /// <summary>
        ///      Grid object
        /// </summary>
        public SGrid<T> Grid
        {
            get { return _source; }
        }
    }
    #endregion
}