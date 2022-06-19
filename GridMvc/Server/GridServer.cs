using GridCore.Pagination;
using GridCore.Server;
using GridShared;
using GridShared.Columns;
using GridShared.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace GridMvc.Server

{
    /// <summary>
    ///     Grid adapter for html helper
    /// </summary>
    public class GridServer<T> : GridCoreServer<T>
    {
        public GridServer(IEnumerable<T> items, QueryDictionary<StringValues> query, bool renderOnlyRows,
            string gridName, Action<IGridColumnCollection<T>> columns = null, int? pageSize = null,
            string language = "", string pagerViewName = GridPager.DefaultPagerViewName,
            IColumnBuilder<T> columnBuilder = null)
        {
            _source = new SGrid<T>(items, query, renderOnlyRows, pagerViewName, columnBuilder);
            _source.RenderOptions.GridName = gridName;
            columns?.Invoke(_source.Columns);
            if (!string.IsNullOrWhiteSpace(language))
                _source.Language = language;
            if (pageSize.HasValue)
                WithPaging(pageSize.Value);
        }

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

        public GridServer(IEnumerable<T> items, QueryDictionary<string> query, bool renderOnlyRows,
            string gridName, Action<IGridColumnCollection<T>> columns = null, int? pageSize = null,
            string language = "", string pagerViewName = GridPager.DefaultPagerViewName,
            IColumnBuilder<T> columnBuilder = null)
        {
            _source = new SGrid<T>(items, query, renderOnlyRows, pagerViewName, columnBuilder);
            _source.RenderOptions.GridName = gridName;
            columns?.Invoke(_source.Columns);
            if (!string.IsNullOrWhiteSpace(language))
                _source.Language = language;
            if (pageSize.HasValue)
                WithPaging(pageSize.Value);
        }
    }
}