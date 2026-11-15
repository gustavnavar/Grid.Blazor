using GridCore.Pagination;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GridMvc.Tests.Pagination
{
    [TestClass]
    public class GridPagerTests
    {
        private GridPager _pager;

        [TestInitialize]
        public void Init()
        {
            var repo = new TestRepository();
            var grid = new TestGrid(repo.GetAll());
            _pager = new GridPager(grid);
        }

        [TestMethod]
        public void PagerPageCountTest()
        {
            _pager.ItemsCount = 1200;
            _pager.PageSize = 13;

            Assert.AreEqual(93, _pager.PageCount);
        }

        [TestMethod]
        public void PagerDisplayingPagesTest()
        {
            _pager.ItemsCount = 1200;
            _pager.PageSize = 13;

            _pager.MaxDisplayedPages = 5;
            _pager.CurrentPage = 40;

            Assert.AreEqual("_GridPager", _pager.TemplateName);
            Assert.AreEqual(38, _pager.StartDisplayedPage);
            Assert.AreEqual(42, _pager.EndDisplayedPage);
        }

        [TestMethod]
        public void PagerCurrentPageTest()
        {
            _pager.ItemsCount = 1200;
            _pager.PageSize = 13;
            _pager.CurrentPage = 1000;

            Assert.AreEqual(_pager.PageCount, _pager.CurrentPage);
        }


        [TestMethod]
        public void PagerChangePageSizeCountTest()
        {
            _pager.ItemsCount = 1200;
            _pager.PageSize = 13;

            _pager.ChangePageSize = true;
            _pager.QueryPageSize = 20;

            Assert.AreEqual(60, _pager.PageCount);
        }

        [TestMethod]
        public void PagerChangePageSizeDisplayingPagesTest()
        {
            _pager.ItemsCount = 1200;
            _pager.PageSize = 13;

            _pager.MaxDisplayedPages = 5;
            _pager.CurrentPage = 40;

            _pager.ChangePageSize = true;
            _pager.QueryPageSize = 20;

            Assert.AreEqual("_GridPager", _pager.TemplateName);
            Assert.AreEqual(60, _pager.PageCount);
            Assert.AreEqual(38, _pager.StartDisplayedPage);
            Assert.AreEqual(42, _pager.EndDisplayedPage);
        }

        [TestMethod]
        public void PagerChangePageSizeCurrentPageTest()
        {
            _pager.ItemsCount = 1200;
            _pager.PageSize = 13;
            _pager.CurrentPage = 1000;

            _pager.ChangePageSize = true;
            _pager.QueryPageSize = 20;

            Assert.AreEqual(_pager.PageCount, _pager.CurrentPage);
        }
    }
}
