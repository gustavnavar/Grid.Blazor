using GridShared;
using GridMvc.Columns;
using GridMvc.Filtering;
using GridMvc.Sorting;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;

namespace GridMvc.Tests.Renderers
{
    /// <summary>
    /// Summary description for SortTests
    /// </summary>
    [TestClass]
    public class RendererTests
    {
        private IQueryCollection _query;
        private TestRepository _repo;

        [TestInitialize]
        public void Init()
        {
            _query = (new DefaultHttpContext()).Request.Query;
            _repo = new TestRepository();
        }

        [TestMethod]
        public void TestGridHeaderRenderer()
        {
            var renderer = new GridHeaderRenderer();
            var column = new GridColumn<TestModel, string>(c => c.Title, null);
            var htmlstring = renderer.Render(column);
            Assert.IsNotNull(htmlstring);
            string html;
            using (var sw = new StringWriter())
            {
                htmlstring.WriteTo(sw, HtmlEncoder.Default);
                html = sw.ToString();
            };
            Assert.IsTrue(!string.IsNullOrWhiteSpace(html));
            Assert.IsTrue(html.Contains("<th"));
            Assert.IsTrue(html.Contains("class=\"grid-header\""));
        }

        [TestMethod]
        public void TestGridCellRenderer()
        {
            var testGrid = new TestGrid(_repo.GetAll());
            var renderer = new GridCellRenderer();
            var column = new GridColumn<TestModel, string>(c => c.Title, null);
            
            var htmlstring = renderer.Render(column, testGrid.GridItems.First());

            Assert.IsNotNull(htmlstring);
            string html;
            using (var sw = new StringWriter())
            {
                htmlstring.WriteTo(sw, HtmlEncoder.Default);
                html = sw.ToString();
            };
            Assert.IsTrue(!string.IsNullOrWhiteSpace(html));

            Assert.IsTrue(html.Contains("<td"));
            Assert.IsTrue(html.Contains(">A1 test</td>"));
            Assert.IsTrue(html.Contains("class=\"grid-cell\""));
            Assert.IsTrue(html.Contains("data-name=\"Title\""));
        }

        [TestMethod]
        public void TestGridFilterHeaderRenderer()
        {
            var settings = new QueryStringFilterSettings(_query);
            var renderer = new QueryStringFilterColumnHeaderRenderer(settings);

            var column = new GridColumn<TestModel, string>(c => c.Title, new TestGrid(Enumerable.Empty<TestModel>()));

            var htmlstring = renderer.Render(column);
            Assert.IsNotNull(htmlstring);
            string html;
            using (var sw = new StringWriter())
            {
                htmlstring.WriteTo(sw, HtmlEncoder.Default);
                html = sw.ToString();
            };
            Assert.IsTrue(string.IsNullOrEmpty(html));

            column.Filterable(true);

            htmlstring = renderer.Render(column);
            Assert.IsNotNull(htmlstring);
            using (var sw = new StringWriter())
            {
                htmlstring.WriteTo(sw, HtmlEncoder.Default);
                html = sw.ToString();
            };

            Assert.IsTrue(!string.IsNullOrWhiteSpace(html));

            Assert.IsTrue(html.Contains("data-filterdata="));
            Assert.IsTrue(html.Contains("class=\"grid-filter\""));
            Assert.IsTrue(html.Contains("class=\"grid-filter-btn\""));
            Assert.IsTrue(html.Contains("data-widgetdata="));
        }

        [TestMethod]
        public void TestGridSortHeaderRenderer()
        {
            var settings = new QueryStringSortSettings(_query);
            var renderer = new QueryStringSortColumnHeaderRenderer(settings);

            var column = new GridColumn<TestModel, string>(c => c.Title, new TestGrid(Enumerable.Empty<TestModel>()));

            var htmlstring = renderer.Render(column);
            Assert.IsNotNull(htmlstring);
            string html;
            using (var sw = new StringWriter())
            {
                htmlstring.WriteTo(sw, HtmlEncoder.Default);
                html = sw.ToString();
            };
            Assert.IsTrue(!html.Contains("<a"));
            Assert.IsTrue(html.Contains("<span"));

            column.Sortable(true);

            htmlstring = renderer.Render(column);
            Assert.IsNotNull(htmlstring);
            using (var sw = new StringWriter())
            {
                htmlstring.WriteTo(sw, HtmlEncoder.Default);
                html = sw.ToString();
            };

            Assert.IsTrue(!string.IsNullOrWhiteSpace(html));
            Assert.IsTrue(html.Contains("<a"));

            Assert.IsTrue(html.Contains("class=\"grid-header-title\""));
        }
    }
}