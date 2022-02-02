using GridMvc.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GridMvc.Tests.Html
{
    [TestClass]
    public class HtmlOptionsTests
    {
        private TestGrid _grid;
        private GridHtmlOptions<TestModel> _opt;

        [TestInitialize]
        public void Init()
        {
            _grid = new TestGrid(Enumerable.Empty<TestModel>());
            var htmlHelper = new Mock<IHtmlHelper>();
            _opt = new GridHtmlOptions<TestModel>(htmlHelper.Object, _grid, "_Grid");
        }

        [TestMethod]
        public void TestMainMethods()
        {
            _opt.WithPaging(5);
            Assert.IsTrue(_grid.EnablePaging);
            Assert.AreEqual(_grid.Pager.PageSize, 5);

            _opt.WithMultipleFilters();
            Assert.IsTrue(_grid.RenderOptions.AllowMultipleFilters);

            _opt.Searchable();
            Assert.IsTrue(_grid.SearchOptions.Enabled);
            Assert.IsTrue(_grid.SearchOptions.OnlyTextColumns);

            _opt.Named("test");
            Assert.AreEqual(_grid.RenderOptions.GridName, "test");

            _opt.Selectable(true);
            Assert.IsTrue(_grid.RenderOptions.Selectable);
        }
    }

    internal class MyViewEngine : IViewEngine
    {
        public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
        {
            return ViewEngineResult.Found("Grid", new MyView());
        }

        public ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage)
        {
            throw new NotImplementedException();
        }
    }

    internal class MyView : IView
    {
        public string Path => "Grid";


        public async Task RenderAsync(ViewContext context)
        {
            await Task.Delay(10);
            context.Writer.Write(true);
        }
    }
}
