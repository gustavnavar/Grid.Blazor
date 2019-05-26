using GridShared;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GridMvc.Tests.Searching
{
    [TestClass]
    public class SearchTests
    {
        private TestGrid _grid;
        private TestRepository _repo;

        [TestInitialize]
        public void Init()
        {
            _repo = new TestRepository();
        }

        [TestMethod]
        public void TestTextColumnsSearch()
        {
            Dictionary<string, StringValues> store = new Dictionary<string, StringValues>();
            store.Add("grid-search", "TEST");
            QueryCollection query = new QueryCollection(store);
            _grid = new TestGrid(_repo.GetAll(), query);
            _grid.SearchingEnabled = true;
            _grid.SearchingOnlyTextColumns = true;

            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(o => o.Id);
                c.Add(o => o.Title);
                c.Add(o => o.Created);
                c.Add(o => o.Int16Field);
                c.Add(o => o.UInt16Field);
                c.Add(o => o.UInt32Field);
                c.Add(o => o.UInt64Field);
            };
            columns(_grid.Columns);

            var searched = _grid.GridItems;

            var original = _repo.GetAll().AsQueryable().Where(t => t.Title.ToUpper().Contains("TEST"));

            for (int i = 0; i < searched.Count(); i++)
            {
                if (searched.ElementAt(i).Id != original.ElementAt(i).Id)
                    Assert.Fail("Searching not works");
            }
            Assert.AreEqual(_grid.ItemsCount, 3);
        }

        [TestMethod]
        public void TestAllColumnsSearch()
        {
            Dictionary<string, StringValues> store = new Dictionary<string, StringValues>();
            store.Add("grid-search", "3");
            QueryCollection query = new QueryCollection(store);
            _grid = new TestGrid(_repo.GetAll(), query);
            _grid.SearchingEnabled = true;
            _grid.SearchingOnlyTextColumns = false;

            Action<IGridColumnCollection<TestModel>> columns = c =>
            {
                c.Add(o => o.Id);
                c.Add(o => o.Title);
                c.Add(o => o.Created);
                c.Add(o => o.Int16Field);
                c.Add(o => o.UInt16Field);
                c.Add(o => o.UInt32Field);
                c.Add(o => o.UInt64Field);
            };
            columns(_grid.Columns);

            var searched = _grid.GridItems;

            var original = _repo.GetAll().AsQueryable().Where(t => t.Id.ToString().ToUpper().Contains("3")
                || t.Title.ToUpper().Contains("3")
                || t.Created.ToString().ToUpper().Contains("3")
                || t.Int16Field.ToString().ToUpper().Contains("3")
                || t.UInt16Field.ToString().ToUpper().Contains("3")
                || t.UInt32Field.ToString().ToUpper().Contains("3")
                || t.UInt64Field.ToString().ToUpper().Contains("3"));

            for (int i = 0; i < searched.Count(); i++)
            {
                if (searched.ElementAt(i).Id != original.ElementAt(i).Id)
                    Assert.Fail("Searching not works");
            }
            Assert.AreEqual(_grid.ItemsCount, 7);
        }
    }
}