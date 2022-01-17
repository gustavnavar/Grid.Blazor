using GridShared;
using GridShared.Columns;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;

namespace GridBlazor
{
    /// <summary>
    ///     Grid adapter for OData client
    /// </summary>
    public class GridODataClient<T> : GridClient<T>
    {
        public GridODataClient(HttpClient httpClient, string url, IQueryDictionary<StringValues> query, bool renderOnlyRows,
            string gridName, Action<IGridColumnCollection<T>> columns = null, int? pageSize = null, 
            CultureInfo cultureInfo = null, IEnumerable<string> oDataExpandList = null,
            IColumnBuilder<T> columnBuilder = null)
            : base (httpClient, url, query, renderOnlyRows, gridName, columns, cultureInfo, columnBuilder)
        {
            UseServerAPI(ServerAPI.OData);
            if (pageSize.HasValue)
                WithPaging(pageSize.Value);
            if (oDataExpandList != null)
                UseODataExpand(oDataExpandList);
        }

        public GridODataClient(HttpClient httpClient, string url, IMemoryDataService<T> memoryDataService, 
            IQueryDictionary<StringValues> query, bool renderOnlyRows, string gridName, 
            Action<IGridColumnCollection<T>> columns = null, int? pageSize = null,
            CultureInfo cultureInfo = null, IEnumerable<string> oDataExpandList = null,
            IColumnBuilder<T> columnBuilder = null)
            : base(httpClient, url, memoryDataService, query, renderOnlyRows, gridName, columns, cultureInfo, columnBuilder)
        {
            UseServerAPI(ServerAPI.OData);
            if (pageSize.HasValue)
                WithPaging(pageSize.Value);
            if (oDataExpandList != null)
                UseODataExpand(oDataExpandList);
        }
    }
}