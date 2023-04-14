using GridBlazor.Pagination;
using GridShared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace GridBlazor.Tests.Pagination
{
    [TestClass]
    public class GridPagerTests
    {
        private GridPager _pager;

        [TestInitialize]
        public void Init()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Title);
            };
            var repo = new TestRepository();
            var grid = new TestGrid((q) => repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            _pager = new GridPager(grid);
        }

        [TestMethod]
        public void PagerPageCountTest()
        {
            _pager.ItemsCount = 1200;
            _pager.PageSize = 13;

            Assert.AreEqual(_pager.PageCount, 93);
        }

        [TestMethod]
        public void PagerDisplayingPagesTest()
        {
            _pager.ItemsCount = 1200;
            _pager.PageSize = 13;

            _pager.MaxDisplayedPages = 5;
            _pager.CurrentPage = 40;

            Assert.AreEqual(_pager.StartDisplayedPage, 38);
            Assert.AreEqual(_pager.EndDisplayedPage, 42);
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

            Assert.AreEqual(_pager.PageCount, 60);
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

            Assert.AreEqual(_pager.PageCount, 60);
            Assert.AreEqual(_pager.StartDisplayedPage, 38);
            Assert.AreEqual(_pager.EndDisplayedPage, 42);
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
