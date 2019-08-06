using GridShared;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GridBlazor.Tests.GridState
{
    [TestClass]
    public class GridStateTests
    {
        private IGridClient<TestModel> _client;
        private TestRepository _repo;

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
            QueryDictionary<StringValues> query = new QueryDictionary<StringValues>();
            _repo = new TestRepository();
            _client = new GridClient<TestModel>((q) => _repo.GetAllService(_columns, q, true, true),
                query, false, "testGrid", _columns);
        }

        [TestMethod]
        public void TestMainMethods()
        {
            _client.WithPaging(5);
            string gridState = _client.Grid.GetState();
            var query = StringExtensions.GetQuery(gridState);
            foreach (var element in query)
            {
                Assert.IsTrue(_client.Grid.Query.ContainsKey(element.Key));
                Assert.AreEqual(_client.Grid.Query[element.Key], element.Value);
            }

            query = new QueryDictionary<StringValues>();
            query.Add("grid-search", "TEST");
            query.Add("grid-page", "2");
            _client = new GridClient<TestModel>((q) => _repo.GetAllService(_columns, q, true, true),
                query, false, "testGrid", _columns);
            _client.Searchable();
            gridState = _client.Grid.GetState();
            query = StringExtensions.GetQuery(gridState);
            foreach (var element in query)
            {
                Assert.IsTrue(_client.Grid.Query.ContainsKey(element.Key));
                Assert.AreEqual(_client.Grid.Query[element.Key], element.Value);
            }
        }
    }
}