using GridShared.Sorting;
using GridMvc.Filtering;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using GridMvc.Searching;

namespace GridMvc.Tests.Sorting
{
    /// <summary>
    /// Summary description for SortTests
    /// </summary>
    [TestClass]
    public class SortTests
    {
        private TestGrid _grid;
        private TestRepository _repo;
        private IQueryCollection _query;


        [TestInitialize]
        public void Init()
        {
            _query = (new DefaultHttpContext()).Request.Query;

            _repo = new TestRepository();
            _grid = new TestGrid(_repo.GetAll());
        }

        [TestMethod]
        public void TestSortingStringDescending()
        {
            _grid.Columns.Add(x => x.Title).Sortable(true);
            if (
                !ValidateSorting<string,  object>(_grid, x => x.Title,  "Title", GridSortDirection.Descending, null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingStringDescendingWithCustomColumnInternalName()
        {
            _grid.Columns.Add(x => x.Title, "someid").Sortable(true);
            if (
                !ValidateSorting<string,  object>(_grid, x => x.Title, "someid",
                                                         GridSortDirection.Descending, null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingStringAscending()
        {
            _grid.Columns.Add(x => x.Title).Sortable(true);
            if (
                !ValidateSorting<string,  object>(_grid, x => x.Title, "Title",
                                                         GridSortDirection.Ascending, null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingStringAscendingWithCustomColumnInternalName()
        {
            _grid.Columns.Add(x => x.Title, "someid").Sortable(true);
            if (
                !ValidateSorting<string,  object>(_grid, x => x.Title, "someid",
                                                         GridSortDirection.Ascending, null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingIntAscending()
        {
            _grid.Columns.Add(x => x.Id).Sortable(true);
            if (
                !ValidateSorting<int,  object>(_grid, x => x.Id,"Id", GridSortDirection.Ascending, null,
                                                   null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingIntDescending()
        {
            _grid.Columns.Add(x => x.Id).Sortable(true);
            if (
                !ValidateSorting<int,  object>(_grid, x => x.Id, "Id", GridSortDirection.Descending, null,
                                                   null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildStringAscending()
        {
            _grid.Columns.Add(x => x.Child.ChildTitle).Sortable(true);
            if (
                !ValidateSorting<string,  object>(_grid, x => x.Child.ChildTitle, "Child.ChildTitle", GridSortDirection.Ascending, null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildStringDescending()
        {
            _grid.Columns.Add(x => x.Child.ChildTitle).Sortable(true);
            if (
                !ValidateSorting<string, object>(_grid, x => x.Child.ChildTitle, 
                                                         "Child.ChildTitle", GridSortDirection.Descending, null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildDateTimeDescending()
        {
            _grid.Columns.Add(x => x.Child.ChildCreated).Sortable(true);
            if (
                !ValidateSorting<DateTime,  object>(_grid, x => x.Child.ChildCreated, 
                                                             "Child.ChildCreated", GridSortDirection.Descending, null,
                                                             null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildDateTimeDescendingWithCustomInternalColumnName()
        {
            _grid.Columns.Add(x => x.Child.ChildCreated, "someid").Sortable(true);
            if (
                !ValidateSorting<DateTime, object>(_grid, x => x.Child.ChildCreated,
                                                             "someid", GridSortDirection.Descending, null,
                                                             null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildDateTimeAscending()
        {
            _grid.Columns.Add(x => x.Child.ChildCreated).Sortable(true);
            if (
                !ValidateSorting<DateTime, object>(_grid, x => x.Child.ChildCreated,
                                                             "Child.ChildCreated", GridSortDirection.Ascending, null,
                                                             null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingThenByAscending()
        {
            _grid.Columns.Add(x => x.Child.ChildCreated).Sortable(true).ThenSortBy(x => x.Title);
            if (
                !ValidateSorting(_grid, x => x.Child.ChildCreated,"Child.ChildCreated",
                                 GridSortDirection.Ascending, x => x.Title, GridSortDirection.Ascending))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingThenByDescending()
        {
            _grid.Columns.Add(x => x.Child.ChildCreated).Sortable(true).ThenSortByDescending(x => x.Title);
            if (
                !ValidateSorting(_grid, x => x.Child.ChildCreated, "Child.ChildCreated",
                                 GridSortDirection.Ascending, x => x.Title, GridSortDirection.Descending))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        private bool ValidateSorting<T, TNext>(TestGrid grid, Func<TestModel, T> orderExpression,
                                                        string columnName,
                                                        GridSortDirection direction,
                                                        Func<TestModel, TNext> thenByExpression,
                                                        GridSortDirection? thenByDirection)
        {
            var settingsMock = new Mock<IGridSettingsProvider>();
            settingsMock.Setup(s => s.SortSettings.ColumnName).Returns(columnName);
            settingsMock.Setup(s => s.SortSettings.Direction).Returns(direction);
            settingsMock.Setup(s => s.FilterSettings).Returns(new QueryStringFilterSettings(_query));
            settingsMock.Setup(s => s.SearchSettings).Returns(new QueryStringSearchSettings(_query));
            grid.Settings = settingsMock.Object;

            IEnumerable<TestModel> resultCollection = _grid.GetItemsToDisplay();
            IOrderedEnumerable<TestModel> etalonCollection;
            switch (direction)
            {
                case GridSortDirection.Ascending:
                    etalonCollection = _repo.GetAll().OrderBy(orderExpression);
                    break;
                case GridSortDirection.Descending:
                    etalonCollection = _repo.GetAll().OrderByDescending(orderExpression);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction");
            }
            if (thenByExpression != null)
            {
                switch (thenByDirection)
                {
                    case GridSortDirection.Ascending:
                        etalonCollection = etalonCollection.ThenBy(thenByExpression);
                        break;
                    case GridSortDirection.Descending:
                        etalonCollection = etalonCollection.ThenByDescending(thenByExpression);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("thenByDirection");
                }
            }

            if (!ValidateCollectionsTheSame(resultCollection, etalonCollection))
            {
                return false;
            }
            return true;
        }

        private bool ValidateCollectionsTheSame<T>(IEnumerable<T> collection1, IEnumerable<T> collection2)
        {
            for (int i = 0; i < collection1.Count(); i++)
            {
                if (!collection1.ElementAt(i).Equals(collection2.ElementAt(i)))
                    return false;
            }
            return true;
        }
    }
}