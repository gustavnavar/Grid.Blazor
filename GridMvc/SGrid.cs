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
using GridShared.Pagination;
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
            : this(items, GridExtensions.Convert(query), renderOnlyRows, pagerViewName, columnBuilder)
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
            if (PagingType != PagingType.Virtualization)
            {
                ((GridPager)_pager).TemplateName = pagerViewName;
            }

            RenderOptions.RenderRowsOnly = renderOnlyRows;
        }

        
        public SGrid(IEnumerable<T> items, IQueryCollection query, IColumnBuilder<T> columnBuilder = null)
            : this(items, GridExtensions.Convert(query), columnBuilder)
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

            var urlParameters = CustomQueryStringBuilder.Convert(query);

            int page = 0;
            int startIndex = 0;
            int virtualizedCount = 0;

            string startIndexParameter = urlParameters[GridPager.DefaultStartIndexQueryParameter];
            string virtualizedCountParameter = urlParameters[GridPager.DefaultVirtualizedCountQueryParameter];
            if (!string.IsNullOrEmpty(startIndexParameter) && !string.IsNullOrWhiteSpace(virtualizedCountParameter))
            {
                PagingType = PagingType.Virtualization;
                int.TryParse(startIndexParameter, out startIndex);
                int.TryParse(virtualizedCountParameter, out virtualizedCount);
                ((GridPager)Pager).StartIndex = startIndex;
                ((GridPager)Pager).VirtualizedCount = virtualizedCount;
            }
            else
            {
                PagingType = PagingType.Pagination;
                string pageParameter = urlParameters[((GridPager)Pager).ParameterName];
                if (pageParameter != null)
                    int.TryParse(pageParameter, out page);
                if (page == 0)
                    page++;
                ((GridPager)Pager).CurrentPage = page;
            }
        }
    }
}