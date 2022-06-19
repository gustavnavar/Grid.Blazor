using GridCore.Pagination;
using GridCore.Resources;
using GridShared;
using GridShared.Columns;
using GridShared.Pagination;
using GridShared.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GridCore.Server

{
    /// <summary>
    ///     Grid adapter for html helper
    /// </summary>
    public class GridCoreServer<T> : IGridServer<T>
    {
        protected internal ISGrid<T> _source;

        public GridCoreServer()
        { }

        public GridCoreServer(IEnumerable<T> items, QueryDictionary<StringValues> query, bool renderOnlyRows,
            string gridName, Action<IGridColumnCollection<T>> columns = null, int? pageSize = null,
            string language = "", string pagerViewName = GridPager.DefaultPagerViewName,
            IColumnBuilder<T> columnBuilder = null)
        {
            _source = new SGridCore<T>(items, query, renderOnlyRows, pagerViewName, columnBuilder);
            _source.RenderOptions.GridName = gridName;
            columns?.Invoke(_source.Columns);
            if (!string.IsNullOrWhiteSpace(language))
                _source.Language = language;
            if (pageSize.HasValue)
                WithPaging(pageSize.Value);
        }

        public GridCoreServer(IEnumerable<T> items, IQueryCollection query, bool renderOnlyRows,
            string gridName, Action<IGridColumnCollection<T>> columns = null, int? pageSize = null,
            string language = "", string pagerViewName = GridPager.DefaultPagerViewName,
            IColumnBuilder<T> columnBuilder = null)
        {
            _source = new SGridCore<T>(items, query, renderOnlyRows, pagerViewName, columnBuilder);
            _source.RenderOptions.GridName = gridName;
            columns?.Invoke(_source.Columns);
            if (!string.IsNullOrWhiteSpace(language))
                _source.Language = language;
            if (pageSize.HasValue)
                WithPaging(pageSize.Value);
        }

        public GridCoreServer(IEnumerable<T> items, QueryDictionary<string> query, bool renderOnlyRows,
            string gridName, Action<IGridColumnCollection<T>> columns = null, int? pageSize = null,
            string language = "", string pagerViewName = GridPager.DefaultPagerViewName,
            IColumnBuilder<T> columnBuilder = null)
        {
            _source = new SGridCore<T>(items, query, renderOnlyRows, pagerViewName, columnBuilder);
            _source.RenderOptions.GridName = gridName;
            columns?.Invoke(_source.Columns);
            if (!string.IsNullOrWhiteSpace(language))
                _source.Language = language;
            if (pageSize.HasValue)
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
            return Searchable(o =>
            {
                o.Enabled = enable;
                o.OnlyTextColumns = onlyTextColumns;
                o.HiddenColumns = hiddenColumns;
                o.SplittedWords = false;
            });
        }

        public IGridServer<T> Searchable(Action<SearchOptions> searchOptions)
        {
            var options = new SearchOptions();
            searchOptions?.Invoke(options);

            _source.SearchOptions = options;
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

        public IGridServer<T> ExtSortable(bool enable, bool hidden)
        {
            _source.ExtSortingEnabled = enable;
            _source.HiddenExtSortingHeader = hidden;
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

        public IGridServer<T> Groupable(bool enable, bool hidden)
        {
            _source.ExtSortingEnabled = enable;
            _source.GroupingEnabled = enable;
            _source.HiddenExtSortingHeader = hidden;
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

        public IGridServer<T> SetDirection(GridDirection dir)
        {
            _source.Direction = dir;
            return this;
        }

        public IGridServer<T> SetTableLayout(TableLayout tableLayout, string width = null, string height = null)
        {
            _source.TableLayout = tableLayout;
            if (!string.IsNullOrWhiteSpace(width))
                _source.Width = width;
            if (!string.IsNullOrWhiteSpace(height))
                _source.Height = height;
            return this;
        }

        public IGridServer<T> SetRemoveDiacritics<R>(string methodName)
        {
            MethodInfo removeDiacritics = typeof(R).GetMethod(methodName, new[] { typeof(string) });
            _source.RemoveDiacritics = removeDiacritics;
            return this;
        }

        public IGridServer<T> SetToListAsyncFunc(Func<IQueryable<T>, Task<IList<T>>> toListAsync)
        {
            _source.SetToListAsyncFunc(toListAsync);
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

        public async Task<ItemsDTO<T>> GetItemsToDisplayAsync(Func<IQueryable<T>, Task<IList<T>>> toListAsync)
        {
            var items = await _source.GetItemsToDisplayAsync(toListAsync);
            var totals = _source.GetTotals();
            return new ItemsDTO<T>(items, totals, new PagerDTO(_source.EnablePaging, _source.Pager.PageSize,
                _source.Pager.CurrentPage, _source.ItemsCount));
        }

        /// <summary>
        ///      Grid object
        /// </summary>
        public ISGrid<T> Grid
        {
            get { return _source; }
        }
    }
    #endregion
}