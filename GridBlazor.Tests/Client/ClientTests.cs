using GridShared;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GridBlazor.Tests.Client
{
    [TestClass]
    public class ClientTests
    {
        private GridClient<TestModel> _client;

        [TestInitialize]
        public void Init()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(r => r.Id);
                c.Add(r => r.Title);
                c.Add(r => r.Created);
                c.Add(r => r.Child);
                c.Add(r => r.List);
                c.Add(r => r.Int16Field);
                c.Add(r => r.UInt16Field);
                c.Add(r => r.UInt32Field);
                c.Add(r => r.UInt64Field);
            };

            _client = new GridClient<TestModel>(null, "", new QueryDictionary<StringValues>(),
                false, "_Grid", columns);
        }

        [TestMethod]
        public void TestMainMethods()
        {
            _client.WithPaging(5);
            Assert.IsTrue(_client.Grid.EnablePaging);
            Assert.AreEqual(_client.Grid.Pager.PageSize, 5);

            _client.WithMultipleFilters();
            Assert.IsTrue(_client.Grid.ComponentOptions.AllowMultipleFilters);

            _client.Searchable();
            Assert.IsTrue(_client.Grid.SearchingEnabled);
            Assert.IsTrue(_client.Grid.SearchingOnlyTextColumns);

            _client.Named("test");
            Assert.AreEqual(_client.Grid.ComponentOptions.GridName, "test");

            _client.Selectable(true);
            Assert.IsTrue(_client.Grid.ComponentOptions.Selectable);
  
        }
    }
}
