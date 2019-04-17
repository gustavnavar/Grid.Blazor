using GridShared;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using System;
using System.Globalization;

namespace GridBlazor.Tests
{
    public class TestGrid : CGrid<TestModel>
    {
        public TestGrid(string url, bool renderOnlyRows, Action<IGridColumnCollection<TestModel>> columns, 
            CultureInfo cultureInfo)
            : base(url, new QueryDictionary<StringValues>(), renderOnlyRows, columns, cultureInfo)
        {
        }

        public TestGrid(Func<QueryDictionary<StringValues>, ItemsDTO<TestModel>> dataService,
            bool renderOnlyRows, Action<IGridColumnCollection<TestModel>> columns, CultureInfo cultureInfo) 
            : base(dataService, new QueryDictionary<StringValues>(), renderOnlyRows, columns, cultureInfo)
        {
        }
    }
}