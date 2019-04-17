using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace GridMvc.Tests
{
    public class TestGrid : SGrid<TestModel>
    {
        public TestGrid(IEnumerable<TestModel> items)
            : base(items, (new DefaultHttpContext()).Request.Query)
        {
        }
    }
}