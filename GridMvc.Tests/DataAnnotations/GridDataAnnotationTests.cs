using GridShared.Columns;
using GridShared.DataAnnotations;
using GridMvc.Tests.DataAnnotations.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;

namespace GridMvc.Tests.DataAnnotations
{

    [TestClass]
    public class GridDataAnnotationTests
    {

        private SGrid<TestGridAnnotationModel> _grid;
        [TestInitialize]
        public void Init()
        {
            HttpContext context = new DefaultHttpContext(); 
            _grid = new SGrid<TestGridAnnotationModel>(Enumerable.Empty<TestGridAnnotationModel>().AsQueryable(), context.Request.Query);
        }

        [TestMethod]
        public void TestPaging()
        {
            Assert.AreEqual(_grid.EnablePaging, true);
            Assert.AreEqual(_grid.Pager.PageSize, 20);
        }

        [TestMethod]
        public void TestColumnsDataAnnotation()
        {
            _grid.AutoGenerateColumns();
            int i = 0;
            foreach (var pi in typeof(TestGridAnnotationModel).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (pi.GetAttribute<NotMappedColumnAttribute>() != null)
                    continue;

                var gridOpt = pi.GetAttribute<GridColumnAttribute>();

                if (gridOpt != null)
                {
                    var column = _grid.Columns.ElementAt(i) as IGridColumn<TestGridAnnotationModel>;
                    if (column == null)
                        Assert.Fail();

                    Assert.AreEqual(column.EncodeEnabled, gridOpt.EncodeEnabled);
                    Assert.AreEqual(column.FilterEnabled, gridOpt.FilterEnabled);
                    Assert.AreEqual(column.SanitizeEnabled, gridOpt.SanitizeEnabled);

                    if (!string.IsNullOrEmpty(gridOpt.Title))
                        Assert.AreEqual(column.Title, gridOpt.Title);

                    if (!string.IsNullOrEmpty(gridOpt.Width))
                        Assert.AreEqual(column.Width, gridOpt.Width);
                }
                i++;
            }
            Assert.AreEqual(_grid.Columns.Count(), 3);
        }
    }
}
