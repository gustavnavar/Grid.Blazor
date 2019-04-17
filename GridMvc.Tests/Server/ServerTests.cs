using GridShared;
using GridMvc.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;

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
            _server.WithPaging(5);
            Assert.IsTrue(_server.Grid.EnablePaging);
            Assert.AreEqual(_server.Grid.Pager.PageSize, 5);

            _server.WithMultipleFilters();
            Assert.IsTrue(_server.Grid.RenderOptions.AllowMultipleFilters);

            _server.Named("test");
            Assert.AreEqual(_server.Grid.RenderOptions.GridName, "test");

            _server.Selectable(true);
            Assert.IsTrue(_server.Grid.RenderOptions.Selectable);
        }
    }
}
