using GridShared;
using GridShared.Sorting;
using GridBlazor.Sorting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GridBlazor.Tests.Sorting
{
    /// <summary>
    /// Summary description for SortTests
    /// </summary>
    [TestClass]
    public class SortTests
    {
        private TestRepository _repo;

        [TestInitialize]
        public void Init()
        {
            _repo = new TestRepository();
        }

        [TestMethod]
        public void TestSortingStringDescending()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Title).Sortable(true);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            if (
                !ValidateSorting<string,  object>(grid, x => x.Title,  "Title", GridSortDirection.Descending, 
                    null, null, null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingStringDescendingWithCustomColumnInternalName()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Title, "someid").Sortable(true);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            if (
                !ValidateSorting<string,  object>(grid, x => x.Title, "someid",
                                                         GridSortDirection.Descending, null, null, null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingStringAscending()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Title).Sortable(true);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            if (
                !ValidateSorting<string,  object>(grid, x => x.Title, "Title",
                                                         GridSortDirection.Ascending, null, null, null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingStringAscendingWithCustomColumnInternalName()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Title, "someid").Sortable(true);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            if (
                !ValidateSorting<string,  object>(grid, x => x.Title, "someid",
                                                         GridSortDirection.Ascending, null, null, null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingIntAscending()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Id).Sortable(true);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            if (
                !ValidateSorting<int,  object>(grid, x => x.Id,"Id", GridSortDirection.Ascending, null,
                                                   null, null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingIntDescending()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Id).Sortable(true);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            if (
                !ValidateSorting<int,  object>(grid, x => x.Id, "Id", GridSortDirection.Descending, null,
                                                   null, null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildStringAscending()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Child.ChildTitle).Sortable(true);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            if (
                !ValidateSorting<string,  object>(grid, x => x.Child.ChildTitle, "Child.ChildTitle", GridSortDirection.Ascending, 
                    null, null, null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildStringDescending()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Child.ChildTitle).Sortable(true);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            if (
                !ValidateSorting<string, object>(grid, x => x.Child.ChildTitle, 
                                                         "Child.ChildTitle", GridSortDirection.Descending, 
                                                         null, null, null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildDateTimeDescending()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Child.ChildCreated).Sortable(true);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            if (
                !ValidateSorting<DateTime,  object>(grid, x => x.Child.ChildCreated, 
                                                             "Child.ChildCreated", GridSortDirection.Descending, null,
                                                             null, null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildDateTimeDescendingWithCustomInternalColumnName()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Child.ChildCreated, "someid").Sortable(true);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            if (
                !ValidateSorting<DateTime, object>(grid, x => x.Child.ChildCreated,
                                                             "someid", GridSortDirection.Descending, null,
                                                             null, null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildDateTimeAscending()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Child.ChildCreated).Sortable(true);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            if (
                !ValidateSorting<DateTime, object>(grid, x => x.Child.ChildCreated,
                                                             "Child.ChildCreated", GridSortDirection.Ascending, null,
                                                             null, null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingThenByAscending()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Child.ChildCreated).Sortable(true).ThenSortBy(x => x.Title);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            if (
                !ValidateSorting(grid, x => x.Child.ChildCreated,"Child.ChildCreated",
                                 GridSortDirection.Ascending, x => x.Title, GridSortDirection.Ascending,
                                 null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingThenByDescending()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Child.ChildCreated).Sortable(true).ThenSortByDescending(x => x.Title);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            if (
                !ValidateSorting(grid, x => x.Child.ChildCreated, "Child.ChildCreated",
                                 GridSortDirection.Ascending, x => x.Title, GridSortDirection.Descending,
                                 null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingStringComparerAscending()
        {
            var comparer = new SampleComparer(StringComparer.OrdinalIgnoreCase);
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Title, comparer).Sortable(true);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            if (
                !ValidateSorting<string, object>(grid, x => x.Title, "Title", GridSortDirection.Ascending,
                    null, null, comparer, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingStringComparerDescending()
        {
            var comparer = new SampleComparer(StringComparer.OrdinalIgnoreCase);
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Title, comparer).Sortable(true);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            if (
                !ValidateSorting<string, object>(grid, x => x.Title, "Title", GridSortDirection.Descending,
                    null, null, comparer, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        private bool ValidateSorting<T, TNext>(TestGrid grid, Func<TestModel, T> orderExpression,
                                                        string columnName,
                                                        GridSortDirection direction,
                                                        Func<TestModel, TNext> thenByExpression,
                                                        GridSortDirection? thenByDirection,
                                                        IComparer<T> comparer,
                                                        IComparer<TNext> nextComparer)
        {
            grid.AddQueryParameter(((QueryStringSortSettings)grid.Settings.SortSettings)
                .ColumnQueryParameterName, columnName);
            grid.AddQueryParameter(((QueryStringSortSettings)grid.Settings.SortSettings)
                .DirectionQueryParameterName, direction.ToString("d"));
            grid.UpdateGrid().Wait();

            IEnumerable<TestModel> resultCollection = grid.GetItemsToDisplay();
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
            if (thenByExpression != null)
            {
                switch (thenByDirection)
                {
                    case GridSortDirection.Ascending:
                        if (nextComparer == null)
                            etalonCollection = etalonCollection.ThenBy(thenByExpression);
                        else
                            etalonCollection = etalonCollection.ThenBy(thenByExpression, nextComparer);
                        break;
                    case GridSortDirection.Descending:
                        if (nextComparer == null)
                            etalonCollection = etalonCollection.ThenByDescending(thenByExpression);
                        else
                            etalonCollection = etalonCollection.ThenByDescending(thenByExpression, nextComparer);
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