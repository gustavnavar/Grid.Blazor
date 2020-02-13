using GridShared;
using GridShared.Sorting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GridBlazor.Tests.ExtSorting
{
    /// <summary>
    /// Summary description for ExtSortTests
    /// </summary>
    [TestClass]
    public class ExtSortTests
    {
        private TestRepository _repo;

        [TestInitialize]
        public void Init()
        {
            _repo = new TestRepository();
        }

        [TestMethod]
        public void TestExtSortingStringDescending()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Title);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<string,  object>(grid, x => x.Title,  "Title", GridSortDirection.Descending, 
                    null, null, null, null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingStringDescendingWithCustomColumnInternalName()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Title, "someid");
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<string,  object>(grid, x => x.Title, "someid",
                                                         GridSortDirection.Descending, null, null, null, null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingStringAscending()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Title);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<string,  object>(grid, x => x.Title, "Title",
                                                         GridSortDirection.Ascending, null, null, null, null, null))
            {
                Assert.Fail("Extended sort  works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingStringAscendingWithCustomColumnInternalName()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Title, "someid");
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<string,  object>(grid, x => x.Title, "someid",
                                                         GridSortDirection.Ascending, null, null, null, null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingIntAscending()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Id);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<int,  object>(grid, x => x.Id,"Id", GridSortDirection.Ascending, null,
                                                   null, null, null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingIntDescending()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Id);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<int,  object>(grid, x => x.Id, "Id", GridSortDirection.Descending, null,
                                                   null, null, null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildStringAscending()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Child.ChildTitle);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<string,  object>(grid, x => x.Child.ChildTitle, "Child.ChildTitle", GridSortDirection.Ascending, 
                    null, null, null, null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildStringDescending()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Child.ChildTitle);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<string, object>(grid, x => x.Child.ChildTitle, 
                                                         "Child.ChildTitle", GridSortDirection.Descending, 
                                                         null, null, null, null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildDateTimeDescending()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Child.ChildCreated);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<DateTime,  object>(grid, x => x.Child.ChildCreated, 
                                                             "Child.ChildCreated", GridSortDirection.Descending, null,
                                                             null, null, null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildDateTimeDescendingWithCustomInternalColumnName()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Child.ChildCreated, "someid");
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<DateTime, object>(grid, x => x.Child.ChildCreated,
                                                             "someid", GridSortDirection.Descending, null,
                                                             null, null, null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildDateTimeAscending()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Child.ChildCreated);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<DateTime, object>(grid, x => x.Child.ChildCreated,
                                                             "Child.ChildCreated", GridSortDirection.Ascending, null,
                                                             null, null, null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingThenByAscending()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Child.ChildCreated);
                c.Add(x => x.Title);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting(grid, x => x.Child.ChildCreated,"Child.ChildCreated",
                                 GridSortDirection.Ascending, x => x.Title, "Title", GridSortDirection.Ascending, 
                                 null, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingThenByDescending()
        {
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Child.ChildCreated);
                c.Add(x => x.Title);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting(grid, x => x.Child.ChildCreated, "Child.ChildCreated",
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
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Title, comparer);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<string, object>(grid, x => x.Title, "Title", GridSortDirection.Ascending,
                    null, null, null, comparer, null))
            {
                Assert.Fail("Extended sort works incorrect");
            }
        }

        [TestMethod]
        public void TestExtSortingStringComparerDescending()
        {
            var comparer = new SampleComparer(StringComparer.OrdinalIgnoreCase);
            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(x => x.Title, comparer);
            };
            var grid = new TestGrid((q) => _repo.GetAllService(columns, q, false, true), true, columns, Thread.CurrentThread.CurrentCulture);
            grid.ExtSortingEnabled = true;
            if (
                !ValidateSorting<string, object>(grid, x => x.Title, "Title", GridSortDirection.Descending,
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
            grid.AddQueryString(ColumnOrderValue.DefaultSortingQueryParameter, payload.ToString());

            IEnumerable<TestModel> resultCollection = grid.GetItemsToDisplay();
            IOrderedEnumerable<TestModel> etalonCollection;
            switch (direction)
            {
                case GridSortDirection.Ascending:
                    if (nextComparer == null)
                        etalonCollection = _repo.GetAll().OrderBy(orderExpression);
                    else
                        etalonCollection = _repo.GetAll().OrderBy(orderExpression, comparer);
                    break;
                case GridSortDirection.Descending:
                    if (nextComparer == null)
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
                grid.AddQueryString(ColumnOrderValue.DefaultSortingQueryParameter, payload.ToString());

                switch (direction2.Value)
                {
                    case GridSortDirection.Ascending:
                        if (nextComparer == null)
                            etalonCollection = _repo.GetAll().OrderBy(orderExpression2);
                        else
                            etalonCollection = _repo.GetAll().OrderBy(orderExpression2, nextComparer);
                        break;
                    case GridSortDirection.Descending:
                        if (nextComparer == null)
                            etalonCollection = _repo.GetAll().OrderByDescending(orderExpression2);
                        else
                            etalonCollection = _repo.GetAll().OrderByDescending(orderExpression2, nextComparer);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("direction");
                }
            }
            grid.UpdateGrid().Wait();

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