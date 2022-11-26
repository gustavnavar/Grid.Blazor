using GridCore;
using GridCore.Searching;
using GridCore.Sorting;
using GridShared.Filtering;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace GridMvc.Tests.Filtering
{
    [TestClass]
    public class FilterTests
    {
        private TestGrid _grid;
        private TestRepository _repo;
        private IQueryDictionary<StringValues> _query;

        [TestInitialize]
        public void Init()
        {
            _query = new QueryDictionary<StringValues>();
            _repo = new TestRepository();
            _grid = new TestGrid(_repo.GetAll());
        }

        [TestMethod]
        public void TestFilter()
        {
            var filterOption = new ColumnFilterValue
                {
                    ColumnName = "Created",
                    FilterType = GridFilterType.LessThan,
                    FilterValue = "10.05.2005"
                };
            var filterOptions = new List<ColumnFilterValue>();
            filterOptions.Add(filterOption);
            var filter = new DefaultColumnFilter<TestModel, DateTime>(m => m.Created);

            var filtered = filter.ApplyFilter(_repo.GetAll().AsQueryable(), filterOptions);

            var original = _repo.GetAll().AsQueryable().Where(t => t.Created < new DateTime(2005, 5, 10));

            for (int i = 0; i < filtered.Count(); i++)
            {
                if (filtered.ElementAt(i).Id != original.ElementAt(i).Id)
                    Assert.Fail("Filtering not works");
            }

            //var processed processor.Process()
        }

        [TestMethod]
        public void TestFilterLessOrEquals()
        {
            var filterOption = new ColumnFilterValue
            {
                ColumnName = "Created",
                FilterType = GridFilterType.LessThanOrEquals,
                FilterValue = "2002-05-01"
            };
            var filterOptions = new List<ColumnFilterValue>();
            filterOptions.Add(filterOption);
            var filter = new DefaultColumnFilter<TestModel, DateTime>(m => m.Created);

            var filtered = filter.ApplyFilter(_repo.GetAll().AsQueryable(), filterOptions);

            var original = _repo.GetAll().AsQueryable().Where(t => t.Created <= new DateTime(2002, 5, 1));

            for (int i = 0; i < filtered.Count(); i++)
            {
                if (filtered.ElementAt(i).Id != original.ElementAt(i).Id)
                    Assert.Fail("Filtering not works");
            }
        }

        [TestMethod]
        public void TestFilterGreaterOrEquals()
        {
            var filterOption = new ColumnFilterValue
            {
                ColumnName = "Created",
                FilterType = GridFilterType.GreaterThanOrEquals,
                FilterValue = "2002-05-01"
            };
            var filterOptions = new List<ColumnFilterValue>();
            filterOptions.Add(filterOption);
            var filter = new DefaultColumnFilter<TestModel, DateTime>(m => m.Created);

            var filtered = filter.ApplyFilter(_repo.GetAll().AsQueryable(), filterOptions);

            var original = _repo.GetAll().AsQueryable().Where(t => t.Created >= new DateTime(2002, 5, 1));

            for (int i = 0; i < filtered.Count(); i++)
            {
                if (filtered.ElementAt(i).Id != original.ElementAt(i).Id)
                    Assert.Fail("Filtering not works");
            }
        }

        [TestMethod]
        public void TestFilterContains()
        {
            var filterOption = new ColumnFilterValue
            {
                ColumnName = "Title",
                FilterType = GridFilterType.Contains,
                FilterValue = "test"
            };
            var filterOptions = new List<ColumnFilterValue>();
            filterOptions.Add(filterOption);
            var filter = new DefaultColumnFilter<TestModel, string>(m => m.Title);

            var filtered = filter.ApplyFilter(_repo.GetAll().AsQueryable(), filterOptions);

            var original = _repo.GetAll().AsQueryable().Where(t => t.Title.Contains("test"));

            for (int i = 0; i < filtered.Count(); i++)
            {
                if (filtered.ElementAt(i).Id != original.ElementAt(i).Id)
                    Assert.Fail("Filtering not works");
            }
        }

        [TestMethod]
        public void TestInt16Filtering()
        {
            var filterOption = new ColumnFilterValue
            {
                ColumnName = "Int16Field",
                FilterType = GridFilterType.Equals,
                FilterValue = "16"
            };
            var filterOptions = new List<ColumnFilterValue>();
            filterOptions.Add(filterOption);
            var filter = new DefaultColumnFilter<TestModel, Int16>(m => m.Int16Field);

            var filtered = filter.ApplyFilter(_repo.GetAll().AsQueryable(), filterOptions);

            var original = _repo.GetAll().AsQueryable().Where(t => t.Int16Field == 16);

            for (int i = 0; i < filtered.Count(); i++)
            {
                if (filtered.ElementAt(i).Id != original.ElementAt(i).Id)
                    Assert.Fail("Filtering not works");
            }
        }

        [TestMethod]
        public void TestUInt16Filtering()
        {
            var filterOption = new ColumnFilterValue
            {
                ColumnName = "UInt16Field",
                FilterType = GridFilterType.Equals,
                FilterValue = "16"
            };
            var filterOptions = new List<ColumnFilterValue>();
            filterOptions.Add(filterOption);
            var filter = new DefaultColumnFilter<TestModel, UInt16>(m => m.UInt16Field);

            var filtered = filter.ApplyFilter(_repo.GetAll().AsQueryable(), filterOptions);

            var original = _repo.GetAll().AsQueryable().Where(t => t.UInt16Field == 16);

            for (int i = 0; i < filtered.Count(); i++)
            {
                if (filtered.ElementAt(i).Id != original.ElementAt(i).Id)
                    Assert.Fail("Filtering not works");
            }
        }

        [TestMethod]
        public void TestUInt32Filtering()
        {
            var filterOption = new ColumnFilterValue
            {
                ColumnName = "UInt32Field",
                FilterType = GridFilterType.Equals,
                FilterValue = "65549"
            };
            var filterOptions = new List<ColumnFilterValue>();
            filterOptions.Add(filterOption);
            var filter = new DefaultColumnFilter<TestModel, UInt32>(m => m.UInt32Field);

            var filtered = filter.ApplyFilter(_repo.GetAll().AsQueryable(), filterOptions);

            var original = _repo.GetAll().AsQueryable().Where(t => t.UInt32Field == 65549);

            for (int i = 0; i < filtered.Count(); i++)
            {
                if (filtered.ElementAt(i).Id != original.ElementAt(i).Id)
                    Assert.Fail("Filtering not works");
            }
        }

        [TestMethod]
        public void TestUInt64Filtering()
        {
            var filterOption = new ColumnFilterValue
            {
                ColumnName = "UInt64Field",
                FilterType = GridFilterType.Equals,
                FilterValue = "4294967888"
            };
            var filterOptions = new List<ColumnFilterValue>();
            filterOptions.Add(filterOption);
            var filter = new DefaultColumnFilter<TestModel, UInt64>(m => m.UInt64Field);

            var filtered = filter.ApplyFilter(_repo.GetAll().AsQueryable(), filterOptions);

            var original = _repo.GetAll().AsQueryable().Where(t => t.UInt64Field == 4294967888);

            for (int i = 0; i < filtered.Count(); i++)
            {
                if (filtered.ElementAt(i).Id != original.ElementAt(i).Id)
                    Assert.Fail("Filtering not works");
            }
        }

        [TestMethod]
        public void TestGuidFilteringEquals()
        {
            var filterOption = new ColumnFilterValue
            {
                ColumnName = "GuidField",
                FilterType = GridFilterType.Equals,
                FilterValue = "6e4fe7c4-a5cb-4e29-8041-a80ce17ea727"
            };
            var filterOptions = new List<ColumnFilterValue>();
            filterOptions.Add(filterOption);
            var filter = new DefaultColumnFilter<TestModel, Guid>(m => m.GuidField);

            var filtered = filter.ApplyFilter(_repo.GetAll().AsQueryable(), filterOptions);

            var original = _repo.GetAll().AsQueryable().Where(t => t.GuidField == new Guid("6e4fe7c4-a5cb-4e29-8041-a80ce17ea727"));

            for (int i = 0; i < filtered.Count(); i++)
            {
                if (filtered.ElementAt(i).Id != original.ElementAt(i).Id)
                    Assert.Fail("Filtering not works");
            }
        }

        [TestMethod]
        public void TestEnumFilteringEquals()
        {
            var filterOption = new ColumnFilterValue
            {
                ColumnName = "EnumField",
                FilterType = GridFilterType.Equals,
                FilterValue = "Foo"
            };
            var filterOptions = new List<ColumnFilterValue>();
            filterOptions.Add(filterOption);
            var filter = new DefaultColumnFilter<TestModel, TestEnum>(m => m.EnumField);

            var filtered = filter.ApplyFilter(_repo.GetAll().AsQueryable(), filterOptions);

            var original = _repo.GetAll().AsQueryable().Where(t => t.EnumField == TestEnum.Foo);

            for (int i = 0; i < filtered.Count(); i++)
            {
                if (filtered.ElementAt(i).Id != original.ElementAt(i).Id)
                    Assert.Fail("Filtering not works");
            }
        }

        [TestMethod]
        public void TestGuidFilterContains()
        {
            var filterOption = new ColumnFilterValue
            {
                ColumnName = "GuidField",
                FilterType = GridFilterType.Contains,
                FilterValue = "0ce17ea"
            };
            var filterOptions = new List<ColumnFilterValue>();
            filterOptions.Add(filterOption);
            var filter = new DefaultColumnFilter<TestModel, Guid>(m => m.GuidField);

            var filtered = filter.ApplyFilter(_repo.GetAll().AsQueryable(), filterOptions);

            var original = _repo.GetAll().AsQueryable().Where(t => t.GuidField.ToString().ToUpper().Contains("0ce17ea".ToUpper()));

            for (int i = 0; i < filtered.Count(); i++)
            {
                if (filtered.ElementAt(i).Id != original.ElementAt(i).Id)
                    Assert.Fail("Filtering not works");
            }
        }

        [TestMethod]
        public void TestGuidFilterStartsWith()
        {
            var filterOption = new ColumnFilterValue
            {
                ColumnName = "GuidField",
                FilterType = GridFilterType.StartsWith,
                FilterValue = "6e4fe7c4"
            };
            var filterOptions = new List<ColumnFilterValue>();
            filterOptions.Add(filterOption);
            var filter = new DefaultColumnFilter<TestModel, Guid>(m => m.GuidField);

            var filtered = filter.ApplyFilter(_repo.GetAll().AsQueryable(), filterOptions);

            var original = _repo.GetAll().AsQueryable().Where(t => t.GuidField.ToString().ToUpper().StartsWith("6e4fe7c4".ToUpper()));

            for (int i = 0; i < filtered.Count(); i++)
            {
                if (filtered.ElementAt(i).Id != original.ElementAt(i).Id)
                    Assert.Fail("Filtering not works");
            }
        }

        [TestMethod]
        public void TestFilteringDateTimeLessThan()
        {
            var filterValue = new DateTime(2005, 5, 10);
            var settings = MockFilterSetting("Created", filterValue.Date.ToString(CultureInfo.InvariantCulture), GridFilterType.LessThan);
            TestFiltering(settings, x => x.Created, x => x.Created < filterValue);
        }

        [TestMethod]
        public void TestFilteringDateTimeLessThanWithCustomInternalColumnName()
        {
            var filterValue = new DateTime(2005, 5, 10);
            var settings = MockFilterSetting("someid", filterValue.ToString("d"), GridFilterType.LessThan);
            _grid.Columns.Add(x => x.Created, "someid").Filterable(true);
            if (!ValidateFiltering(_grid, settings, x => x.Created < filterValue))
            {
                Assert.Fail("Filtering works incorrect");
            }
        }

        [TestMethod]
        public void TestFilteringStringEquals()
        {
            var firstItem = _repo.GetAll().First();
            var settings = MockFilterSetting("Title", firstItem.Title, GridFilterType.Contains);
            TestFiltering(settings, x => x.Title, x => x.Title.ToUpper() == firstItem.Title.ToUpper());
        }

        [TestMethod]
        public void TestFilteringStringEqualsCaseInsensative()
        {
            var firstItem = _repo.GetAll().First();
            var settings = MockFilterSetting("Title", firstItem.Title.ToUpper(), GridFilterType.Contains);
            TestFiltering(settings, x => x.Title, x => x.Title.ToUpper() == firstItem.Title.ToUpper());
        }

        [TestMethod]
        public void TestFilteringStringContains()
        {
            var firstItem = _repo.GetAll().First();
            var settings = MockFilterSetting("Title", firstItem.Title, GridFilterType.Contains);
            TestFiltering(settings, x => x.Title, x => x.Title.ToUpper().Contains(firstItem.Title.ToUpper()));
        }

        [TestMethod]
        public void TestFilteringIntEquals()
        {
            var firstItem = _repo.GetAll().First();
            var settings = MockFilterSetting("Id", firstItem.Title, GridFilterType.Contains);
            TestFiltering(settings, x => x.Title, x => x.Id == firstItem.Id);
        }

        [TestMethod]
        public void TestFilteringChildEquals()
        {
            var firstItem = _repo.GetAll().First();
            var settings = MockFilterSetting("Created2", firstItem.Child.ChildCreated.Date.ToString(CultureInfo.InvariantCulture), GridFilterType.Equals);
            TestFiltering(settings, x => x.Child.ChildCreated, x => x.Child.ChildCreated == firstItem.Child.ChildCreated);
        }

        private void TestFiltering<T>(ColumnFilterValue settings, Expression<Func<TestModel, T>> column,
                                   Func<TestModel, bool> filterContraint)
        {
            _grid.Columns.Add(column, settings.ColumnName).Filterable(true);
            if (!ValidateFiltering(_grid, settings, filterContraint))
            {
                Assert.Fail("Filtering works incorrect");
            }
        }

        private bool ValidateFiltering(TestGrid grid, ColumnFilterValue value,
                                                        Func<TestModel, bool> filterExpression)
        {
            var settingsMock = new Mock<IGridSettingsProvider>();
            var filterSetting = new Mock<IGridFilterSettings>();
            var filterCollection = new DefaultFilterColumnCollection { value };

            filterSetting.Setup(t => t.FilteredColumns).Returns(filterCollection);
            filterSetting.Setup(t => t.IsInitState(_grid.Columns.First())).Returns(false);

            settingsMock.Setup(s => s.FilterSettings).Returns(filterSetting.Object);
            settingsMock.Setup(s => s.SortSettings).Returns(new QueryStringSortSettings(_query));
            settingsMock.Setup(s => s.SearchSettings).Returns(new QueryStringSearchSettings(_query));
            grid.Settings = settingsMock.Object;

            IEnumerable<TestModel> resultCollection = _grid.GetItemsToDisplay();
            if (!resultCollection.Any()) Assert.Fail("No items to compare");

            IEnumerable<TestModel> etalonCollection = _repo.GetAll().Where(filterExpression);

            if (!ValidateCollectionsTheSame(resultCollection, etalonCollection))
            {
                return false;
            }
            return true;
        }

        private ColumnFilterValue MockFilterSetting(string columnName, string filterValue, GridFilterType type)
        {
            return new ColumnFilterValue
                {
                    ColumnName = columnName,
                    FilterValue = filterValue,
                    FilterType = type
                };
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