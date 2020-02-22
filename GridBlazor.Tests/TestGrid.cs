using GridShared;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using System;
using System.Globalization;
using System.Net.Http;

namespace GridBlazor.Tests
{
    public class TestGrid : CGrid<TestModel>
    {
        public TestGrid(HttpClient httpClient, string url, bool renderOnlyRows, 
            Action<IGridColumnCollection<TestModel>> columns, CultureInfo cultureInfo)
            : base(httpClient, url, new QueryDictionary<StringValues>(), renderOnlyRows, columns, cultureInfo)
        {
        }

        public TestGrid(Func<QueryDictionary<StringValues>, ItemsDTO<TestModel>> dataService,
            bool renderOnlyRows, Action<IGridColumnCollection<TestModel>> columns, CultureInfo cultureInfo) 
            : base(dataService, new QueryDictionary<StringValues>(), renderOnlyRows, columns, cultureInfo)
        {
        }
    }
}