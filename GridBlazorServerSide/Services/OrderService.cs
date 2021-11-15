using GridBlazorServerSide.Data;
using GridBlazorServerSide.Models;
using GridCore.Server;
using GridShared;
using GridShared.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazorServerSide.Services
{
    public class OrderService : IOrderService
    {
        private readonly DbContextOptions<NorthwindDbContext> _options;

        public OrderService(DbContextOptions<NorthwindDbContext> options)
        {
            _options = options;
        }

        public ItemsDTO<Order> GetOrdersGridRows(Action<IGridColumnCollection<Order>> columns,
            QueryDictionary<StringValues> query)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                var repository = new OrdersRepository(context);

                var server = new GridCoreServer<Order>(repository.GetAll(), query, true, "ordersGrid", columns)
                        .Sortable()
                        .WithPaging(10)
                        .Filterable()
                        .WithMultipleFilters()
                        .Groupable(true)                        
                        .Searchable(true, false, false)
                        .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

                // return items to displays
                var items = server.ItemsToDisplay;

                // uncomment the following lines are to test null responses
                //items = null;
                //items.Items = null;
                //items.Pager = null;
                return items;
            }
        }

        public ItemsDTO<Order> GetOrdersGridRowsWithCount(Action<IGridColumnCollection<Order>> columns,
            QueryDictionary<StringValues> query)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                var repository = new OrdersRepository(context);
                var server = new GridCoreServer<Order>(repository.GetAll().Include(r => r.OrderDetails), query, true, "ordersGrid", columns)
                        .Sortable()
                        .WithPaging(10)
                        .Filterable()
                        .WithMultipleFilters()
                        .Groupable(true)
                        .Searchable(true, false, false);

                // return items to displays
                var items = server.ItemsToDisplay;

                // uncomment the following lines are to test null responses
                //items = null;
                //items.Items = null;
                //items.Pager = null;
                return items;
            }
        }

        public ItemsDTO<Order> GetOrdersGridRows(QueryDictionary<StringValues> query)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                var repository = new OrdersRepository(context);
                var server = new GridCoreServer<Order>(repository.GetAll(), query, true, "ordersGrid", null)
                        .AutoGenerateColumns()
                        .Sortable()
                        .WithPaging(10)
                        .Filterable()
                        .WithMultipleFilters()
                        .Groupable(true)
                        .Searchable(true, false, false);

                // return items to displays
                return server.ItemsToDisplay;
            }
        }

        public ItemsDTO<Order> GetOrdersGridRowsInMemory(Action<IGridColumnCollection<Order>> columns,
            QueryDictionary<StringValues> query)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                var repository = new OrdersRepository(context);
                var server = new GridCoreServer<Order>(repository.GetAll().ToList(), query, true, "ordersGrid", columns)
                        .Sortable()
                        .WithPaging(10)
                        .Filterable()
                        .WithMultipleFilters()
                        .Groupable(true)
                        .Searchable(true, false, false);

                // return items to displays
                var items = server.ItemsToDisplay;
                return items;
            }
        }

        public ItemsDTO<Order> GetOrdersWithErrorGridRows(Action<IGridColumnCollection<Order>> columns,
           QueryDictionary<StringValues> query)
        {
            var random = new Random();
            if (random.Next(2) == 0)
                throw new Exception("Random server error");

            using (var context = new NorthwindDbContext(_options))
            {
                var repository = new OrdersRepository(context);
                var server = new GridCoreServer<Order>(repository.GetAll(), query, true, "ordersGrid", columns)
                        .Sortable()
                        .WithPaging(10)
                        .Filterable()
                        .WithMultipleFilters()
                        .Groupable(true)
                        .Searchable(true, false, false);

                // return items to displays
                var items = server.ItemsToDisplay;

                // uncomment the following lines are to test null responses
                //items = null;
                //items.Items = null;
                //items.Pager = null;
                return items;
            }
        }


        public async Task<Order> GetOrder(int OrderId)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                var repository = new OrdersRepository(context);
                return await repository.GetById(OrderId);
            }
        }

        public async Task UpdateAndSave(Order order)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                var repository = new OrdersRepository(context);
                await repository.Update(order);
                repository.Save();
            }
        }

        public async Task Add1ToFreight(int OrderId)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                var repository = new OrdersRepository(context);
                var order =  await repository.GetById(OrderId);
                if (order.Freight.HasValue)
                {
                    order.Freight += 1;
                    await repository.Update(order);
                    repository.Save();
                }
            }
        }

        public async Task Subtract1ToFreight(int OrderId)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                var repository = new OrdersRepository(context);
                var order = await repository.GetById(OrderId);
                if (order.Freight.HasValue)
                {
                    order.Freight -= 1;
                    await repository.Update(order);
                    repository.Save();
                }
            }
        }

        public async Task<Order> Get(params object[] keys)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                int orderId;
                int.TryParse(keys[0].ToString(), out orderId);
                var repository = new OrdersRepository(context);
                return await repository.GetById(orderId);
            }
        }

        public async Task Insert(Order item)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                try
                {
                    var repository = new OrdersRepository(context);
                    await repository.Insert(item);
                    repository.Save();
                }
                catch (Exception e)
                {
                    throw new GridException("ORDSRV-01", e);
                }
            }
        }

        public async Task Update(Order item)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                try
                {
                    var repository = new OrdersRepository(context);
                    await repository.Update(item);
                    repository.Save();
                }
                catch (Exception e)
                {
                    throw new GridException(e);
                }
            }
        }

        public async Task Delete(params object[] keys)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                try
                {
                    var order = await Get(keys);
                    var repository = new OrdersRepository(context);
                    repository.Delete(order);
                    repository.Save();
                }
                catch (Exception)
                {
                    throw new GridException("Error deleting the order");
                }
            }
        }
    }

    public interface IOrderService : ICrudDataService<Order>
    {
        ItemsDTO<Order> GetOrdersGridRows(Action<IGridColumnCollection<Order>> columns, QueryDictionary<StringValues> query);
        ItemsDTO<Order> GetOrdersGridRowsWithCount(Action<IGridColumnCollection<Order>> columns, QueryDictionary<StringValues> query);
        ItemsDTO<Order> GetOrdersGridRows(QueryDictionary<StringValues> query);
        ItemsDTO<Order> GetOrdersGridRowsInMemory(Action<IGridColumnCollection<Order>> columns, QueryDictionary<StringValues> query);
        ItemsDTO<Order> GetOrdersWithErrorGridRows(Action<IGridColumnCollection<Order>> columns, QueryDictionary<StringValues> query);
        Task<Order> GetOrder(int OrderId);
        Task UpdateAndSave(Order order);
        Task Add1ToFreight(int OrderId);
        Task Subtract1ToFreight(int OrderId);
    }
}
