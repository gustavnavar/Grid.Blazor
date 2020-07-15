using GridMvc.Server;
using GridShared;
using GridShared.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GridMvc.Tests.GridState
{
    [TestClass]
    public class GridStateTests
    {
        private GridServer<TestModel> _server;

        private Action<IGridColumnCollection<TestModel>> _columns = c =>
        {
            c.Add().Encoded(false).Sanitized(false).RenderValueAs(o => $"{c.Grid.GetState()}");
            c.Add(model => model.Id);
            c.Add(model => model.Title);
            c.Add(model => model.Created);
            c.Add(model => model.Child);
            c.Add(model => model.Int16Field);
            c.Add(model => model.UInt16Field);
            c.Add(model => model.UInt32Field);
            c.Add(model => model.UInt64Field);
        };

        [TestInitialize]
        public void Init()
        {
            HttpContext context = new DefaultHttpContext();
            var viewContextMock = new Mock<ViewContext>();
            _server = new GridServer<TestModel>(Enumerable.Empty<TestModel>(),
                context.Request.Query, false, "_Grid", _columns, 10);
        }

        [TestMethod]
        public void TestMainMethods()
        {
            _server.WithPaging(5);
            string gridState = _server.Grid.GetState();
            var query = StringExtensions.GetQuery(gridState);
            foreach (var element in query)
            {
                Assert.IsTrue(_server.Grid.Query.ContainsKey(element.Key));
                Assert.AreEqual(_server.Grid.Query[element.Key], element.Value);
            }

            Dictionary<string, StringValues> store = new Dictionary<string, StringValues>();
            store.Add("grid-search", "TEST");
            store.Add("grid-page", "2");
            _server = new GridServer<TestModel>(Enumerable.Empty<TestModel>(),
                new QueryCollection(store), false, "_Grid", _columns, 10);
            _server.Searchable();
            gridState = _server.Grid.GetState();
            query = StringExtensions.GetQuery(gridState);
            foreach (var element in query)
            {
                Assert.IsTrue(_server.Grid.Query.ContainsKey(element.Key));
                Assert.AreEqual(_server.Grid.Query[element.Key], element.Value);
            }
        }
    }
}