using GridBlazor.Pagination;
using GridShared;
using GridShared.Pagination;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace GridBlazor.Tests.Pagination
{
    [TestClass]
    public class GridVirtualizationTests
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
            grid.PagingType = PagingType.Virtualization;
            _pager = new GridPager(grid);
        }

        public void PagerInitTest()
        {
            Assert.AreEqual(_pager.StartIndex, 0);
            Assert.AreEqual(_pager.VirtualizedCount, 0);
        }

        [TestMethod]
        public void PagerStartIndexTest()
        {
            _pager.StartIndex = 3;

            Assert.AreEqual(_pager.StartIndex, 3);
            Assert.AreEqual(_pager.VirtualizedCount, 0);
        }

        [TestMethod]
        public void PagerVirtualCountTest()
        {
            _pager.VirtualizedCount = 13;

            Assert.AreEqual(_pager.StartIndex, 0);
            Assert.AreEqual(_pager.VirtualizedCount, 13);
        }
    }
}
