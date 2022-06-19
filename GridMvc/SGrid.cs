using GridCore;
using GridCore.Columns;
using GridCore.Filtering;
using GridCore.Pagination;
using GridCore.Resources;
using GridCore.Searching;
using GridCore.Sorting;
using GridCore.Totals;
using GridMvc.Columns;
using GridMvc.DataAnnotations;
using GridShared;
using GridShared.Columns;
using GridShared.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace GridMvc
{
    /// <summary>
    ///     Grid.Mvc base class
    /// </summary>
    public class SGrid<T> : SGridCore<T>
    {
        public SGrid()
        { }

        public SGrid(IEnumerable<T> items, IQueryCollection query, bool renderOnlyRows,
            string pagerViewName = GridPager.DefaultPagerViewName, IColumnBuilder<T> columnBuilder = null)
            : this(items, QueryDictionary<StringValues>.Convert(query), renderOnlyRows, pagerViewName, columnBuilder)
        {
        }

        public SGrid(IEnumerable<T> items, QueryDictionary<string> query, bool renderOnlyRows,
            string pagerViewName = GridPager.DefaultPagerViewName, IColumnBuilder<T> columnBuilder = null)
            : this(items, query.ToStringValuesDictionary(), renderOnlyRows, pagerViewName, columnBuilder)
        {
        }

        public SGrid(IEnumerable<T> items, QueryDictionary<StringValues> query, bool renderOnlyRows,
            string pagerViewName = GridPager.DefaultPagerViewName, IColumnBuilder<T> columnBuilder = null)
            : this(items, query, columnBuilder)
        {
            var urlParameters = CustomQueryStringBuilder.Convert(query);
            string pageParameter = urlParameters[((GridPager)Pager).ParameterName];
            int page = 0;
            if (pageParameter != null)
                int.TryParse(pageParameter, out page);
            if (page == 0)
                page++;
            ((GridPager)_pager).CurrentPage = page;
            ((GridPager)_pager).TemplateName = pagerViewName;
            RenderOptions.RenderRowsOnly = renderOnlyRows;
        }

        public SGrid(IEnumerable<T> items, IQueryCollection query, IColumnBuilder<T> columnBuilder = null)
            : this(items, QueryDictionary<StringValues>.Convert(query), columnBuilder)
        { }

        public SGrid(IEnumerable<T> items, QueryDictionary<string> query, IColumnBuilder<T> columnBuilder = null)
            : this(items, query.ToStringValuesDictionary(), columnBuilder)
        { }

        public SGrid(IEnumerable<T> items, QueryDictionary<StringValues> query, IColumnBuilder<T> columnBuilder = null)
            : this()
        {
            BeforeItems = items.AsQueryable();

            #region init default properties

            Query = query;

            //set up sort settings:
            _settings = new QueryStringGridSettingsProvider(Query);

            Sanitizer = new Sanitizer();
            EmptyGridText = Strings.DefaultGridEmptyText;
            Language = Strings.Lang;

            _currentSortItemsProcessor = new SortGridItemsProcessor<T>(this, _settings.SortSettings);
            _currentFilterItemsProcessor = new FilterGridItemsProcessor<T>(this, _settings.FilterSettings);
            _currentSearchItemsProcessor = new SearchGridItemsProcessor<T>(this, _settings.SearchSettings);
            _currentTotalsItemsProcessor = new TotalsGridItemsProcessor<T>(this);
            AddItemsPreProcessor(_currentFilterItemsProcessor);
            AddItemsPreProcessor(_currentSearchItemsProcessor);
            InsertItemsProcessor(0, _currentSortItemsProcessor);
            SetTotalsProcessor(_currentTotalsItemsProcessor);

            #endregion init default properties
            _annotations = new GridAnnotationsProvider();

            //Set up column collection:
            if (columnBuilder == null)
                _columnBuilder = new DefaultColumnBuilder<T>(this, _annotations);
            else
                _columnBuilder = columnBuilder;
            _columnsCollection = new GridColumnCollection<T>(this, _columnBuilder, _settings.SortSettings);
            RenderOptions = new GridRenderOptions();

            ApplyGridSettings();
        }
    }
}