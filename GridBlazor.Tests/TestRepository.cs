using GridShared;
using GridShared.Utility;
using GridMvc.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GridBlazor.Tests
{
    internal class TestRepository
    {
        public IQueryable<TestModel> GetAll()
        {
            return GetTestData().AsQueryable();
        }

        public IQueryable<TestModelChild> GetList(int id)
        {
            var testModel = GetTestData().AsQueryable().SingleOrDefault(r => r.Id == id);
            if (testModel == null)
                return null;
            else
                return testModel.List.AsQueryable();
        }

        public ItemsDTO<TestModel> GetAllService(Action<IGridColumnCollection<TestModel>> columns,
            QueryDictionary<StringValues> query, bool enable, bool onlyTextColumns)
        {
            GridServer<TestModel> server = new GridServer<TestModel>(GetTestData().AsQueryable(),
                new QueryCollection(query), true, "_Grid", columns);

            server.Sortable()
                .Filterable()
                .WithMultipleFilters()
                .Searchable(enable, onlyTextColumns);

            // return items to displays
            return server.ItemsToDisplay;
        }

        private IEnumerable<TestModel> GetTestData()
        {
            yield return
                new TestModel
                {
                    Id = 1,
                    Title = "A1 test",
                    Created = new DateTime(2012, 2, 3),
                    Child = new TestModelChild { ChildTitle = "B1", ChildCreated = new DateTime(1994, 3, 16) },
                    List = new TestModelChild[] {
                            new TestModelChild { ChildTitle = "B1 - 1", ChildCreated = new DateTime(2002, 6, 16) },
                            new TestModelChild { ChildTitle = "B1 - 2", ChildCreated = new DateTime(2002, 7, 16) },
                            new TestModelChild { ChildTitle = "B1 - 3", ChildCreated = new DateTime(2002, 8, 16) }
                        }
                };
            yield return
                new TestModel
                {
                    Id = 1,
                    Title = "C3 test2",
                    Created = new DateTime(1998, 1, 1),
                    Child = new TestModelChild { ChildTitle = "C3", ChildCreated = new DateTime(1998, 1, 1) },
                    List = new TestModelChild[] {
                            new TestModelChild { ChildTitle = "C3 - 1", ChildCreated = new DateTime(2002, 6, 15) },
                            new TestModelChild { ChildTitle = "C3 - 2", ChildCreated = new DateTime(2002, 7, 15) },
                            new TestModelChild { ChildTitle = "C3 - 3", ChildCreated = new DateTime(2002, 8, 15) }
                        }
                };
            yield return
                new TestModel
                {
                    Id = 1,
                    Title = "D3 test 3",
                    Created = new DateTime(2009, 3, 15),
                    Child = new TestModelChild { ChildTitle = "D3", ChildCreated = new DateTime(2011, 2, 7) },
                    List = new TestModelChild[] {
                            new TestModelChild { ChildTitle = "D3 - 1", ChildCreated = new DateTime(2002, 6, 14) },
                            new TestModelChild { ChildTitle = "D3 - 2", ChildCreated = new DateTime(2002, 7, 14) },
                            new TestModelChild { ChildTitle = "D3 - 3", ChildCreated = new DateTime(2002, 8, 14) }
                        }
                };
            yield return
                new TestModel
                {
                    Id = 1,
                    Title = "B2",
                    Created = new DateTime(2003, 1, 3),
                    Child = new TestModelChild { ChildTitle = "B2", ChildCreated = new DateTime(1991, 6, 9), },
                    List = new TestModelChild[] {
                            new TestModelChild { ChildTitle = "B2 - 1", ChildCreated = new DateTime(2002, 6, 1) },
                            new TestModelChild { ChildTitle = "B2 - 2", ChildCreated = new DateTime(2002, 7, 1) },
                            new TestModelChild { ChildTitle = "B2 - 3", ChildCreated = new DateTime(2002, 8, 1) }
                        },
                    Int16Field = 16
                };
            yield return
                new TestModel
                {
                    Id = 1,
                    Title = "D2",
                    Created = new DateTime(2009, 3, 16),
                    Child = new TestModelChild { ChildTitle = "D2", ChildCreated = new DateTime(1994, 3, 16) },
                    List = new TestModelChild[] {
                            new TestModelChild { ChildTitle = "D2 - 1", ChildCreated = new DateTime(2002, 6, 12) },
                            new TestModelChild { ChildTitle = "D2 - 2", ChildCreated = new DateTime(2002, 7, 12) },
                            new TestModelChild { ChildTitle = "D2 - 3", ChildCreated = new DateTime(2002, 8, 12) }
                        }
                };
            yield return
                new TestModel
                {
                    Id = 1,
                    Title = "D1",
                    Created = new DateTime(2010, 9, 15),
                    Child = new TestModelChild { ChildTitle = "D1", ChildCreated = new DateTime(1995, 9, 15) },
                    List = new TestModelChild[] {
                            new TestModelChild { ChildTitle = "D1 - 1", ChildCreated = new DateTime(2002, 6, 2) },
                            new TestModelChild { ChildTitle = "D1 - 2", ChildCreated = new DateTime(2002, 7, 2) },
                            new TestModelChild { ChildTitle = "D1 - 3", ChildCreated = new DateTime(2002, 8, 2) }
                        },
                    Int16Field = 16
                };
            yield return
                new TestModel
                {
                    Id = 1,
                    Title = "X1",
                    Created = new DateTime(2011, 1, 12),
                    Child = new TestModelChild { ChildTitle = "X1", ChildCreated = new DateTime(1995, 9, 15) },
                    List = new TestModelChild[] {
                            new TestModelChild { ChildTitle = "X1 - 1", ChildCreated = new DateTime(2002, 6, 3) },
                            new TestModelChild { ChildTitle = "X1 - 2", ChildCreated = new DateTime(2002, 7, 3) },
                            new TestModelChild { ChildTitle = "X1 - 3", ChildCreated = new DateTime(2002, 8, 3) }
                        },
                    UInt16Field = 16
                };
            yield return
                new TestModel
                {
                    Id = 1,
                    Title = "Y1",
                    Created = new DateTime(2006, 4, 5),
                    Child = new TestModelChild { ChildTitle = "Y1", ChildCreated = new DateTime(1971, 9, 15) },
                    List = new TestModelChild[] {
                            new TestModelChild { ChildTitle = "Y1 - 1", ChildCreated = new DateTime(2002, 6, 4) },
                            new TestModelChild { ChildTitle = "Y1 - 2", ChildCreated = new DateTime(2002, 7, 4) },
                            new TestModelChild { ChildTitle = "Y1 - 3", ChildCreated = new DateTime(2002, 8, 4) }
                        },
                    UInt32Field = 65549
                };
            yield return
                     new TestModel
                     {
                         Id = 1,
                         Title = "ZZ1",
                         Created = new DateTime(2014, 5, 21),
                         Child = new TestModelChild { ChildTitle = "ZZ1", ChildCreated = new DateTime(2014, 5, 22) },
                         List = new TestModelChild[] {
                            new TestModelChild { ChildTitle = "ZZ1 - 1", ChildCreated = new DateTime(2002, 6, 5) },
                            new TestModelChild { ChildTitle = "ZZ1 - 2", ChildCreated = new DateTime(2002, 7, 5) },
                            new TestModelChild { ChildTitle = "ZZ1 - 3", ChildCreated = new DateTime(2002, 8, 5) }
                         },
                         UInt64Field = 4294967888
                     };
            yield return
                new TestModel
                {
                    Id = 1,
                    Title = "C2",
                    Created = new DateTime(2007, 8, 4),
                    Child = new TestModelChild { ChildTitle = "C2", ChildCreated = new DateTime(2007, 8, 4) },
                    List = new TestModelChild[] {
                            new TestModelChild { ChildTitle = "C2 - 1", ChildCreated = new DateTime(2002, 6, 6) },
                            new TestModelChild { ChildTitle = "C2 - 2", ChildCreated = new DateTime(2002, 7, 6) },
                            new TestModelChild { ChildTitle = "C2 - 3", ChildCreated = new DateTime(2002, 8, 6) }
                        }
                };
            yield return
                new TestModel
                {
                    Id = 1,
                    Title = "E1",
                    Created = new DateTime(2012, 4, 11),
                    Child = new TestModelChild { ChildTitle = "E1", ChildCreated = new DateTime(1990, 2, 4) },
                    List = new TestModelChild[] {
                            new TestModelChild { ChildTitle = "E1 - 1", ChildCreated = new DateTime(2002, 6, 7) },
                            new TestModelChild { ChildTitle = "E1 - 2", ChildCreated = new DateTime(2002, 7, 7) },
                            new TestModelChild { ChildTitle = "E1 - 3", ChildCreated = new DateTime(2002, 8, 7) }
                        },
                    GuidField = new Guid("6e4fe7c4-a5cb-4e29-8041-a80ce17ea727")
                };
            yield return
                new TestModel
                {
                    Id = 1,
                    Title = "E3",
                    Created = new DateTime(1993, 2, 21),
                    Child = new TestModelChild { ChildTitle = "E3", ChildCreated = new DateTime(1993, 2, 21) },
                    List = new TestModelChild[] {
                            new TestModelChild { ChildTitle = "E3 - 1", ChildCreated = new DateTime(2002, 6, 8) },
                            new TestModelChild { ChildTitle = "E3 - 2", ChildCreated = new DateTime(2002, 7, 8) },
                            new TestModelChild { ChildTitle = "E3 - 3", ChildCreated = new DateTime(2002, 8, 8) }
                        },
                    GuidField = new Guid("e7c6e4f4-a5cb-4e29-8041-a80ce17ea727")
                };
            yield return
                new TestModel
                {
                    Id = 1,
                    Title = "B1",
                    Created = new DateTime(1997, 2, 26),
                    Child = new TestModelChild { ChildTitle = "A1", ChildCreated = new DateTime(1997, 2, 26) },
                    List = new TestModelChild[] {
                            new TestModelChild { ChildTitle = "A1 - 1", ChildCreated = new DateTime(2002, 6, 9) },
                            new TestModelChild { ChildTitle = "A1 - 2", ChildCreated = new DateTime(2002, 7, 9) },
                            new TestModelChild { ChildTitle = "A1 - 3", ChildCreated = new DateTime(2002, 8, 9) }
                        },
                    GuidField = new Guid("22c6e4f4-a5cb-4e29-8041-a80ce17ea727")
                };
            yield return
                new TestModel
                {
                    Id = 1,
                    Title = "E2",
                    Created = new DateTime(2002, 6, 17),
                    Child = new TestModelChild { ChildTitle = "E2", ChildCreated = new DateTime(2002, 6, 17) },
                    List = new TestModelChild[] {
                            new TestModelChild { ChildTitle = "E2 - 1", ChildCreated = new DateTime(2002, 6, 10) },
                            new TestModelChild { ChildTitle = "E2 - 2", ChildCreated = new DateTime(2002, 7, 10) },
                            new TestModelChild { ChildTitle = "E2 - 3", ChildCreated = new DateTime(2002, 8, 10) }
                        }
                };
            yield return
                new TestModel
                {
                    Id = 1,
                    Title = "C1",
                    Created = new DateTime(2002, 5, 1),
                    Child = new TestModelChild { ChildTitle = "C1", ChildCreated = new DateTime(2002, 5, 1) },
                    List = new TestModelChild[] {
                            new TestModelChild { ChildTitle = "C1 - 1", ChildCreated = new DateTime(2002, 6, 11) },
                            new TestModelChild { ChildTitle = "C1 - 2", ChildCreated = new DateTime(2002, 7, 11) },
                            new TestModelChild { ChildTitle = "C1 - 3", ChildCreated = new DateTime(2002, 8, 11) }
                        }
                };
            yield return
                new TestModel
                {
                    Id = 1,
                    Title = "B3",
                    Created = new DateTime(2002, 2, 5),
                    Child = new TestModelChild { ChildTitle = "B3", ChildCreated = new DateTime(2002, 5, 1) },
                    List = new TestModelChild[] {
                            new TestModelChild { ChildTitle = "B3 - 1", ChildCreated = new DateTime(2002, 6, 12) },
                            new TestModelChild { ChildTitle = "B3 - 2", ChildCreated = new DateTime(2002, 7, 12) },
                            new TestModelChild { ChildTitle = "B3 - 3", ChildCreated = new DateTime(2002, 8, 12) }
                        }
                };
            yield return
                new TestModel
                {
                    Id = 1,
                    Title = "B10",
                    Created = new DateTime(2002, 3, 2),
                    Child = new TestModelChild { ChildTitle = "B10", ChildCreated = new DateTime(2002, 3, 5) },
                    List = new TestModelChild[] {
                            new TestModelChild { ChildTitle = "B10 - 1", ChildCreated = new DateTime(2002, 2, 10) },
                            new TestModelChild { ChildTitle = "B10 - 2", ChildCreated = new DateTime(2002, 3, 10) },
                            new TestModelChild { ChildTitle = "B19 - 3", ChildCreated = new DateTime(2002, 4, 10) }
                        }
                };
            yield return
                new TestModel
                {
                    Id = 1,
                    Title = "",
                    Created = new DateTime(2002, 3, 2),
                    Child = new TestModelChild { ChildTitle = "empty-title", ChildCreated = new DateTime(2002, 3, 5) },
                    List = new TestModelChild[] {
                            new TestModelChild { ChildTitle = "empty-title - 1", ChildCreated = new DateTime(2002, 2, 10) },
                            new TestModelChild { ChildTitle = "empty-title - 2", ChildCreated = new DateTime(2002, 3, 10) },
                            new TestModelChild { ChildTitle = "empty-title - 3", ChildCreated = new DateTime(2002, 4, 10) }
                        }
                };
            yield return
                new TestModel
                {
                    Id = 1,
                    Title = null,
                    Created = new DateTime(2002, 3, 2),
                    Child = new TestModelChild { ChildTitle = "null-title", ChildCreated = new DateTime(2002, 3, 5) },
                    List = new TestModelChild[] {
                            new TestModelChild { ChildTitle = "null-title - 1", ChildCreated = new DateTime(2002, 2, 10) },
                            new TestModelChild { ChildTitle = "null-title - 2", ChildCreated = new DateTime(2002, 3, 10) },
                            new TestModelChild { ChildTitle = "null-title - 3", ChildCreated = new DateTime(2002, 4, 10) }
                        }
                };
        }
    }
}