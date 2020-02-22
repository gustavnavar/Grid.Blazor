using GridShared;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace GridBlazor.Tests.Searching
{
    [TestClass]
    public class SearchTests
    {
        private IGridClient<TestModel> _gridClient;
        private TestRepository _repo;

        private Action<IGridColumnCollection<TestModel>> Columns = c =>
        {
            c.Add(o => o.Id);
            c.Add(o => o.Title);
            c.Add(o => o.Created);
            c.Add(o => o.Int16Field);
            c.Add(o => o.UInt16Field);
            c.Add(o => o.UInt32Field);
            c.Add(o => o.UInt64Field);
        };

        [TestInitialize]
        public void Init()
        {
            _repo = new TestRepository();
        }

        [TestMethod]
        public void TestTextColumnsSearch()
        {
            QueryDictionary<StringValues> query = new QueryDictionary<StringValues>();
            query.Add("grid-search", "TEST");

            _gridClient = new GridClient<TestModel>((q) => _repo.GetAllService(Columns, q, true, true), query, false, 
                "testGrid", Columns)
                .Searchable(true, true);

            _gridClient.UpdateGrid().Wait();

            var searched = _gridClient.Grid.Items;

            var original = _repo.GetAll().AsQueryable().Where(t => t.Title.ToUpper().Contains("TEST"));

            for (int i = 0; i < searched.Count(); i++)
            {
                if (searched.ElementAt(i).Id != original.ElementAt(i).Id)
                    Assert.Fail("Searching not works");
            }
            Assert.AreEqual(_gridClient.Grid.ItemsCount, 3);
        }

        [TestMethod]
        public void TestAllColumnsSearch()
        {
            QueryDictionary<StringValues> query = new QueryDictionary<StringValues>();
            query.Add("grid-search", "3");          

            _gridClient = new GridClient<TestModel>((q) => _repo.GetAllService(Columns, q, true, false), query, false, "testGrid", 
                Columns);
            _gridClient.Searchable(true, false);

            _gridClient.UpdateGrid().Wait();

            var searched = _gridClient.Grid.Items;

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
            Assert.AreEqual(_gridClient.Grid.ItemsCount, 8);
        }
    }
}