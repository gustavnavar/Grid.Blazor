using GridCore.Pagination;
using GridShared.Pagination;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GridMvc.Tests.Pagination
{
    [TestClass]
    public class GridVirtualizationTests
    {
        private GridPager _pager;

        [TestInitialize]
        public void Init()
        {
            var repo = new TestRepository();
            var grid = new TestGrid(repo.GetAll());
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
