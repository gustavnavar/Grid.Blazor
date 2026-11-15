using GridShared;
using GridMvc.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using GridShared.Pagination;

namespace GridMvc.Tests.Server
{
    [TestClass]
    public class ServerTests
    {
        private GridServer<TestModel> _server;

        private Action<IGridColumnCollection<TestModel>> _columns = c =>
        {
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
            _server.Grid.PagingType = PagingType.Virtualization;
            Assert.AreEqual(PagingType.Virtualization, _server.Grid.PagingType);
            Assert.AreEqual(0, _server.Grid.Pager.StartIndex);
            Assert.AreEqual(0, _server.Grid.Pager.VirtualizedCount);

            _server.WithPaging(5);
            Assert.AreEqual(PagingType.Pagination, _server.Grid.PagingType);
            Assert.AreEqual(5, _server.Grid.Pager.PageSize);

            Assert.IsTrue(_server.Grid.Pager.GoToVisibility);
            _server.GoToVisibility(false);
            Assert.IsFalse(_server.Grid.Pager.GoToVisibility);

            _server.WithMultipleFilters();
            Assert.IsTrue(_server.Grid.RenderOptions.AllowMultipleFilters);

            _server.Searchable();
            Assert.IsTrue(_server.Grid.SearchOptions.Enabled);
            Assert.IsTrue(_server.Grid.SearchOptions.OnlyTextColumns);

            _server.Named("test");
            Assert.AreEqual("test", _server.Grid.RenderOptions.GridName);

            _server.Selectable(true);
            Assert.IsTrue(_server.Grid.RenderOptions.Selectable);

            _server.ChangePageSize(true);
            Assert.IsTrue(_server.Grid.Pager.ChangePageSize);

            _server.SyncButton(true);
            Assert.IsTrue(_server.Grid.SyncButtonEnabled);

            _server.ClearFiltersButton(true);
            Assert.IsTrue(_server.Grid.ClearFiltersButtonEnabled);

            _server.ExtSortable(true);
            Assert.IsTrue(_server.Grid.ExtSortingEnabled);

            _server.Groupable(false);
            Assert.IsFalse(_server.Grid.ExtSortingEnabled);
            Assert.IsFalse(_server.Grid.GroupingEnabled);
        }
    }
}
