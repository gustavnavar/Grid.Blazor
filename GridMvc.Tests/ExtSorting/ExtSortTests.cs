using GridCore;
using GridShared.Sorting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GridMvc.Tests.ExtSorting
{
    /// <summary>
    /// Summary description for ExtSortTests
    /// </summary>
    [TestClass]
    public class ExtSortTests
    {
        private TestGrid _grid;
        private TestRepository _repo;

        [TestInitialize]
        public void Init()
        {
            _repo = new TestRepository();
            _grid = new TestGrid(_repo.GetAll());
        }

        [TestMethod]
        public void TestExtSortingStringDescending()
        {
            _grid.Columns.Add(x => x.Title);
            _grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<string,  object>(_grid, x => x.Title,  "Title", GridSortDirection.Descending, null, null, 
                    null, null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingStringDescendingWithCustomColumnInternalName()
        {
            _grid.Columns.Add(x => x.Title, "someid");
            _grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<string,  object>(_grid, x => x.Title, "someid",
                                                         GridSortDirection.Descending, null, null, null, null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingStringAscending()
        {
            _grid.Columns.Add(x => x.Title);
            _grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<string,  object>(_grid, x => x.Title, "Title",
                                                         GridSortDirection.Ascending, null, null, null, null, null))
            {
                Assert.Fail("Extended sort  works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingStringAscendingWithCustomColumnInternalName()
        {
            _grid.Columns.Add(x => x.Title, "someid");
            _grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<string,  object>(_grid, x => x.Title, "someid",
                                                         GridSortDirection.Ascending, null, null, null, null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingIntAscending()
        {
            _grid.Columns.Add(x => x.Id);
            _grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<int,  object>(_grid, x => x.Id,"Id", GridSortDirection.Ascending, null,
                                                   null, null, null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingIntDescending()
        {
            _grid.Columns.Add(x => x.Id);
            _grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<int,  object>(_grid, x => x.Id, "Id", GridSortDirection.Descending, null,
                                                   null, null, null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildStringAscending()
        {
            _grid.Columns.Add(x => x.Child.ChildTitle);
            _grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<string,  object>(_grid, x => x.Child.ChildTitle, "Child.ChildTitle", GridSortDirection.Ascending, 
                    null, null, null, null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildStringDescending()
        {
            _grid.Columns.Add(x => x.Child.ChildTitle);
            _grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<string, object>(_grid, x => x.Child.ChildTitle, 
                                                         "Child.ChildTitle", GridSortDirection.Descending, 
                                                         null, null, null, null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildDateTimeDescending()
        {
            _grid.Columns.Add(x => x.Child.ChildCreated);
            _grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<DateTime,  object>(_grid, x => x.Child.ChildCreated, 
                                                             "Child.ChildCreated", GridSortDirection.Descending, null,
                                                             null, null, null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildDateTimeDescendingWithCustomInternalColumnName()
        {
            _grid.Columns.Add(x => x.Child.ChildCreated, "someid");
            _grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<DateTime, object>(_grid, x => x.Child.ChildCreated,
                                                             "someid", GridSortDirection.Descending, null,
                                                             null, null, null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildDateTimeAscending()
        {
            _grid.Columns.Add(x => x.Child.ChildCreated);
            _grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<DateTime, object>(_grid, x => x.Child.ChildCreated,
                                                             "Child.ChildCreated", GridSortDirection.Ascending, null,
                                                             null, null, null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingThenByAscending()
        {
            _grid.Columns.Add(x => x.Child.ChildCreated);
            _grid.Columns.Add(x => x.Title);
            _grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting(_grid, x => x.Child.ChildCreated,"Child.ChildCreated",
                                 GridSortDirection.Ascending, x => x.Title, "Title", GridSortDirection.Ascending, 
                                 null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingThenByDescending()
        {
            _grid.Columns.Add(x => x.Child.ChildCreated);
            _grid.Columns.Add(x => x.Title);
            _grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting(_grid, x => x.Child.ChildCreated, "Child.ChildCreated",
                                 GridSortDirection.Ascending, x => x.Title, "Title", GridSortDirection.Descending, 
                                 null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestExtSortingStringComparerAscending()
        {
            var comparer = new SampleComparer(StringComparer.OrdinalIgnoreCase);
            _grid.Columns.Add(x => x.Title, comparer);
            _grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<string, object>(_grid, x => x.Title, "Title", GridSortDirection.Ascending, 
                    null, null, null, comparer, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestExtSortingStringComparerDescending()
        {
            var comparer = new SampleComparer(StringComparer.OrdinalIgnoreCase);
            _grid.Columns.Add(x => x.Title, comparer);
            _grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<string, object>(_grid, x => x.Title, "Title", GridSortDirection.Descending,
                    null, null, null, comparer, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        private bool ValidateSorting<T, TNext>(TestGrid grid, Func<TestModel, T> orderExpression,
                                                        string columnName,
                                                        GridSortDirection direction,
                                                        Func<TestModel, TNext> orderExpression2,
                                                        string columnName2,
                                                        GridSortDirection? direction2,
                                                        IComparer<T> comparer,
                                                        IComparer<TNext> nextComparer)
        {
            var payload = new ColumnOrderValue(columnName, direction,
                grid.Settings.SortSettings.SortValues.Count + 1);
            grid.Query.AddParameter(ColumnOrderValue.DefaultSortingQueryParameter, payload.ToString());
            grid.Settings = new QueryStringGridSettingsProvider(grid.Query);
            
            IOrderedEnumerable<TestModel> etalonCollection;
            switch (direction)
            {
                case GridSortDirection.Ascending:
                    if (comparer == null)
                        etalonCollection = _repo.GetAll().OrderBy(orderExpression);
                    else
                        etalonCollection = _repo.GetAll().OrderBy(orderExpression, comparer);
                    break;
                case GridSortDirection.Descending:
                    if (comparer == null)
                        etalonCollection = _repo.GetAll().OrderByDescending(orderExpression);
                    else
                        etalonCollection = _repo.GetAll().OrderByDescending(orderExpression, comparer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction");
            }

            if (orderExpression2 != null && !string.IsNullOrWhiteSpace(columnName2) && direction2.HasValue)
            {
                payload = new ColumnOrderValue(columnName2, direction2.Value,
                grid.Settings.SortSettings.SortValues.Count + 1);
                grid.Query.AddParameter(ColumnOrderValue.DefaultSortingQueryParameter, payload.ToString());
                grid.Settings = new QueryStringGridSettingsProvider(grid.Query);

                switch (direction2.Value)
                {
                    case GridSortDirection.Ascending:
                        if (comparer == null)
                            etalonCollection = _repo.GetAll().OrderBy(orderExpression2);
                        else
                            etalonCollection = _repo.GetAll().OrderBy(orderExpression2, nextComparer);
                        break;
                    case GridSortDirection.Descending:
                        if (comparer == null)
                            etalonCollection = _repo.GetAll().OrderByDescending(orderExpression2);
                        else
                            etalonCollection = _repo.GetAll().OrderByDescending(orderExpression2, nextComparer);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("direction");
                }
            }
            IEnumerable<TestModel> resultCollection = grid.GetItemsToDisplay();

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