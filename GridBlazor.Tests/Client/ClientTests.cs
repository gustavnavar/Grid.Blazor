using GridShared;
using GridShared.Pagination;
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
            _client.Virtualize("1200px",600);
            Assert.AreEqual(PagingType.Virtualization, _client.Grid.PagingType);
            Assert.AreEqual(0, _client.Grid.Pager.StartIndex);
            Assert.AreEqual(0, _client.Grid.Pager.VirtualizedCount);
            Assert.AreEqual(ServerAPI.ItemsDTO, _client.Grid.ServerAPI);
            Assert.AreEqual("1200px", _client.Grid.Width);
            Assert.AreEqual("600px", _client.Grid.Height);

            _client.WithPaging(5);
            Assert.AreEqual(PagingType.Pagination, _client.Grid.PagingType);
            Assert.AreEqual(5, _client.Grid.Pager.PageSize);
            Assert.AreEqual(ServerAPI.ItemsDTO, _client.Grid.ServerAPI);

            Assert.IsTrue(_client.Grid.Pager.GoToVisibility);
            _client.GoToVisibility(false);
            Assert.IsFalse(_client.Grid.Pager.GoToVisibility);

            _client.WithMultipleFilters();
            Assert.IsTrue(_client.Grid.ComponentOptions.AllowMultipleFilters);

            _client.Searchable();
            Assert.IsTrue(_client.Grid.SearchOptions.Enabled);
            Assert.IsTrue(_client.Grid.SearchOptions.OnlyTextColumns);

            _client.Named("test");
            Assert.AreEqual("test", _client.Grid.ComponentOptions.GridName);

            _client.Selectable(true);
            Assert.IsTrue(_client.Grid.ComponentOptions.Selectable);

            _client.ChangePageSize(true);
            Assert.IsTrue(_client.Grid.Pager.ChangePageSize);

            _client.SetModalForms(true, "auto", "auto");
            Assert.IsTrue(_client.Grid.ModalForms);
            Assert.AreEqual("auto", _client.Grid.ModalWidth);
            Assert.AreEqual("auto", _client.Grid.ModalHeight);

            _client.SyncButton(true);
            Assert.IsTrue(_client.Grid.SyncButtonEnabled);

            _client.ClearFiltersButton(true);
            Assert.IsTrue(_client.Grid.ClearFiltersButtonEnabled);

            _client.ExtSortable(true);
            Assert.IsTrue(_client.Grid.ExtSortingEnabled);

            _client.Groupable(false);
            Assert.IsFalse(_client.Grid.ExtSortingEnabled);
            Assert.IsFalse(_client.Grid.GroupingEnabled);

            _client.RearrangeableColumns(true);
            Assert.IsTrue(_client.Grid.RearrangeColumnEnabled);
        }
    }
}
