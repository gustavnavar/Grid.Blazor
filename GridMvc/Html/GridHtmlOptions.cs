using GridCore;
using GridCore.Pagination;
using GridCore.Resources;
using GridShared;
using GridShared.Columns;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace GridMvc.Html
{
    /// <summary>
    ///     Grid adapter for html helper
    /// </summary>
    public class GridHtmlOptions<T> : IGridHtmlOptions<T>
    {
        private readonly IHtmlHelper _helper;
        internal readonly SGrid<T> _source;

        public GridHtmlOptions(IHtmlHelper helper, SGrid<T> source, string viewName)
        {
            _helper = helper;
            _source = source;
            GridViewName = viewName;
        }

        public string GridViewName { get; set; }

        #region IGridHtmlOptions<T> Members

        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            _helper.PartialAsync(GridViewName, _source).Result.WriteTo(writer, encoder);
        }

        public IGridHtmlOptions<T> WithGridItemsCount()
        {
            return WithGridItemsCount(string.Empty);
        }

        public string Render()
        {
            using (var sw = new StringWriter())
            {
                WriteTo(sw, HtmlEncoder.Default);
                return sw.ToString();
            }
        }

        public async Task<IHtmlContent> RenderAsync()
        {
            return await _helper.PartialAsync(GridViewName, _source);
        }

        public IGridHtmlOptions<T> Columns(Action<IGridColumnCollection<T>> columnBuilder)
        {
            columnBuilder(_source.Columns);
            return this;
        }

        public IGridHtmlOptions<T> ChangePageSize(bool enable)
        {
            _source.Pager.ChangePageSize = enable;
            return this;
        }

        public IGridHtmlOptions<T> WithPaging(int pageSize)
        {
            return WithPaging(pageSize, 0);
        }

        public IGridHtmlOptions<T> WithPaging(int pageSize, int maxDisplayedItems)
        {
            return WithPaging(pageSize, maxDisplayedItems, string.Empty);
        }

        public IGridHtmlOptions<T> WithPaging(int pageSize, int maxDisplayedItems, string queryStringParameterName)
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

        public IGridHtmlOptions<T> Sortable()
        {
            return Sortable(true);
        }

        public IGridHtmlOptions<T> Sortable(bool enable)
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

        public IGridHtmlOptions<T> Filterable()
        {
            return Filterable(true);
        }

        public IGridHtmlOptions<T> Filterable(bool enable)
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

        public IGridHtmlOptions<T> Searchable()
        {
            return Searchable(true, true);
        }

        public IGridHtmlOptions<T> Searchable(bool enable)
        {
            return Searchable(enable, true);
        }

        public IGridHtmlOptions<T> Searchable(bool enable, bool onlyTextColumns)
        {
            return Searchable(enable, onlyTextColumns, false);
        }

        public IGridHtmlOptions<T> Searchable(bool enable, bool onlyTextColumns, bool hiddenColumns)
        {
            return Searchable(o =>
            {
                o.Enabled = enable;
                o.OnlyTextColumns = onlyTextColumns;
                o.HiddenColumns = hiddenColumns;
                o.SplittedWords = false;
            });
        }

        public IGridHtmlOptions<T> Searchable(Action<SearchOptions> searchOptions)
        {
            var options = new SearchOptions();
            searchOptions?.Invoke(options);

            _source.SearchOptions = options;
            return this;
        }

        public IGridHtmlOptions<T> ExtSortable()
        {
            return ExtSortable(true);
        }

        public IGridHtmlOptions<T> ExtSortable(bool enable)
        {
            _source.ExtSortingEnabled = enable;
            return this;
        }

        public IGridHtmlOptions<T> ExtSortable(bool enable, bool hidden)
        {
            _source.ExtSortingEnabled = enable;
            _source.HiddenExtSortingHeader = hidden;
            return this;
        }

        public IGridHtmlOptions<T> Groupable()
        {
            return Groupable(true);
        }

        public IGridHtmlOptions<T> Groupable(bool enable)
        {
            _source.ExtSortingEnabled = enable;
            _source.GroupingEnabled = enable;
            return this;
        }

        public IGridHtmlOptions<T> Groupable(bool enable, bool hidden)
        {
            _source.ExtSortingEnabled = enable;
            _source.GroupingEnabled = enable;
            _source.HiddenExtSortingHeader = hidden;
            return this;
        }

        public IGridHtmlOptions<T> ClearFiltersButton(bool enable)
        {
            _source.ClearFiltersButtonEnabled = enable;
            return this;
        }

        public IGridHtmlOptions<T> Selectable(bool set)
        {
            _source.RenderOptions.Selectable = set;
            return this;
        }

        public IGridHtmlOptions<T> EmptyText(string text)
        {
            _source.EmptyGridText = text;
            return this;
        }

        public IGridHtmlOptions<T> SetLanguage(string lang)
        {
            _source.Language = lang;
            return this;
        }

        public IGridHtmlOptions<T> SetRowCssClasses(Func<T, string> contraint)
        {
            _source.SetRowCssClassesContraint(contraint);
            return this;
        }

        public IGridHtmlOptions<T> Named(string gridName)
        {
            _source.RenderOptions.GridName = gridName;
            return this;
        }

        /// <summary>
        ///     Generates columns for all properties of the model.
        ///     Use data annotations to customize columns
        /// </summary>
        public IGridHtmlOptions<T> AutoGenerateColumns()
        {
            _source.AutoGenerateColumns();
            return this;
        }

        public IGridHtmlOptions<T> WithMultipleFilters()
        {
            _source.RenderOptions.AllowMultipleFilters = true;
            return this;
        }

        /// <summary>
        ///     Set to true if we want to show grid itmes count
        ///     - Author - Jeeva J
        /// </summary>
        public IGridHtmlOptions<T> WithGridItemsCount(string gridItemsName)
        {
            if (string.IsNullOrWhiteSpace(gridItemsName))
                gridItemsName = Strings.Items;

            _source.RenderOptions.GridCountDisplayName = gridItemsName;
            _source.RenderOptions.ShowGridItemsCount = true;
            return this;
        }

        public IGridHtmlOptions<T> SetStriped(bool enabled)
        {
            _source.RenderOptions.Striped = enabled;
            return this;
        }

        /// <summary>
        ///    Allow grid to show a SubGrid
        /// </summary>
        public IGridHtmlOptions<T> SubGrid(params string[] keys)
        {
            _source.SubGridKeys = keys;
            return this;
        }

        public IGridHtmlOptions<T> SetDirection(GridDirection dir)
        {
            _source.Direction = dir;
            return this;
        }

        public IGridHtmlOptions<T> SetTableLayout(TableLayout tableLayout, string width = null, string height = null)
        {
            _source.TableLayout = tableLayout;
            if (!string.IsNullOrWhiteSpace(width))
                _source.Width = width;
            if (!string.IsNullOrWhiteSpace(height))
                _source.Height = height;
            return this;
        }

        public GridRenderOptions RenderOptions
        {
            get { return _source.RenderOptions; }
        }

        public IGridPager Pager
        {
            get { return _source.Pager; }
        }

        public IGridSettingsProvider Settings
        {
            get { return _source.Settings; }
        }

        #endregion
    }
}