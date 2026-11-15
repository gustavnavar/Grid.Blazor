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
            Assert.AreEqual(0, _pager.StartIndex);
            Assert.AreEqual(0, _pager.VirtualizedCount);
        }

        [TestMethod]
        public void PagerStartIndexTest()
        {
            _pager.StartIndex = 3;

            Assert.AreEqual(3, _pager.StartIndex);
            Assert.AreEqual(0, _pager.VirtualizedCount);
        }

        [TestMethod]
        public void PagerVirtualCountTest()
        {
            _pager.VirtualizedCount = 13;

            Assert.AreEqual(0, _pager.StartIndex);
            Assert.AreEqual(13, _pager.VirtualizedCount);
        }
    }
}
