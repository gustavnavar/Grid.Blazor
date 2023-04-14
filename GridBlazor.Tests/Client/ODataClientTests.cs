using GridShared;
using GridShared.Pagination;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GridBlazor.Tests.Client
{
    [TestClass]
    public class ODataClientTests
    {
        private GridODataClient<TestModel> _client;

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

            _client = new GridODataClient<TestModel>(null, "", new QueryDictionary<StringValues>(),
                false, "_Grid", columns, 10);
        }

        [TestMethod]
        public void TestMainMethods()
        {
            Assert.AreEqual(_client.Grid.PagingType, PagingType.Pagination);
            Assert.AreEqual(_client.Grid.ServerAPI, ServerAPI.OData);
            Assert.AreEqual(_client.Grid.Pager.PageSize, 10);

            _client.WithPaging(5);
            Assert.AreEqual(_client.Grid.PagingType, PagingType.Pagination);
            Assert.AreEqual(_client.Grid.Pager.PageSize, 5);

            _client.WithMultipleFilters();
            Assert.IsTrue(_client.Grid.ComponentOptions.AllowMultipleFilters);

            _client.Searchable();
            Assert.IsTrue(_client.Grid.SearchOptions.Enabled);
            Assert.IsTrue(_client.Grid.SearchOptions.OnlyTextColumns);

            _client.Named("test");
            Assert.AreEqual(_client.Grid.ComponentOptions.GridName, "test");

            _client.Selectable(true);
            Assert.IsTrue(_client.Grid.ComponentOptions.Selectable);

            _client.ChangePageSize(true);
            Assert.IsTrue(_client.Grid.Pager.ChangePageSize);

            _client.ClearFiltersButton(true);
            Assert.IsTrue(_client.Grid.ClearFiltersButtonEnabled);

            _client.ExtSortable(true);
            Assert.IsTrue(_client.Grid.ExtSortingEnabled);

            _client.Groupable(false);
            Assert.IsFalse(_client.Grid.ExtSortingEnabled);
            Assert.IsFalse(_client.Grid.GroupingEnabled);
        }
    }
}
