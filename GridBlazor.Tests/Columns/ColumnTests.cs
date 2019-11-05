using GridShared;
using GridBlazor.Columns;
using GridBlazor.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;

namespace GridBlazor.Tests.Columns
{
    [TestClass]
    public class ColumnTests
    {
        private TestGrid _grid;
        private GridColumnCollection<TestModel> _columns;

        [TestInitialize]
        public void Init()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(r => r.Id);
            };
            var repo = new TestRepository();
            _grid = new TestGrid((q) => repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);

            _columns = new GridColumnCollection<TestModel>(_grid, new DefaultColumnBuilder<TestModel>(_grid, new GridAnnotationsProvider()), _grid.Settings.SortSettings);
        }

        [TestMethod]
        public void TestColumnsRetriveByMemberExpression()
        {
            var addedColumn = _columns.Add(x => x.Created);
            var column = _columns.Get(x => x.Created);

            Assert.AreEqual(addedColumn, column);
        }

        [TestMethod]
        public void TestColumnsRetriveByName()
        {
            var addedColumn = _columns.Add(x => x.Created);
            var column = _columns.GetByName("Created");

            Assert.AreEqual(addedColumn, column);
        }


        [TestMethod]
        public void TestRenderingEmptyValueIfNullReferenceOccurs()
        {
            var addedColumn = _columns.Add(x => x.Child.ChildTitle);

            var cell = addedColumn.GetCell(new TestModel
            {
                Child = null
            });

            Assert.AreEqual(cell.Value, string.Empty);
        }

        [TestMethod]
        public void TestColumnsRetriveByNameWithCustomName()
        {
            var addedColumn = _columns.Add(x => x.Created, "My_Column");
            var column = _columns.GetByName("My_Column");

            Assert.AreEqual(addedColumn, column);
        }

        [TestMethod]
        public void TestColumnsCollection()
        {
            _columns.Add();
            _columns.Add();

            _columns.Add(x => x.List[0].ChildCreated);
            _columns.Add(x => x.List[1].ChildCreated, "t1");

            _columns.Add(x => x.Id);
            Assert.AreEqual(_columns.Count(), 5);
            try
            {
                _columns.Add(x => x.Id);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            _columns.Insert(0, x => x.Title);
            Assert.AreEqual(_columns.Count(), 6);
            Assert.AreEqual(_columns.ElementAt(0).Name, "Title");
            //test hidden columns

            _columns.Add(x => x.Created, true);
            Assert.AreEqual(_columns.Count(), 7);
        }

        [TestMethod]
        public void TestColumnInternalNameSetup()
        {
            const string name = "MyId";
            var column = _columns.Add(x => x.Id, name);
            Assert.AreEqual(column.Name, name);
        }
    }
}