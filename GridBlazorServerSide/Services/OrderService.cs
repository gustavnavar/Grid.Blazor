using GridBlazorServerSide.Data;
using GridBlazorServerSide.Models;
using GridMvc.Server;
using GridShared;
using GridShared.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
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
                var server = new GridServer<Order>(repository.GetAll(), new QueryCollection(query),
                    true, "ordersGrid", columns)
                        .Sortable()
                        .WithPaging(10)
                        .Filterable()
                        .WithMultipleFilters()
                        .Groupable(true)
                        .Searchable(true, false);

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
                var server = new GridServer<Order>(repository.GetAll(), new QueryCollection(query),
                    true, "ordersGrid", null).AutoGenerateColumns();

                // return items to displays
                return server.ItemsToDisplay;
            }
        }

        public ItemsDTO<OrderDetail> GetOrderDetailsGridRows(Action<IGridColumnCollection<OrderDetail>> columns,
            object[] keys, QueryDictionary<StringValues> query)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                int orderId;
                int.TryParse(keys[0].ToString(), out orderId);
                var repository = new OrderDetailsRepository(context);
                var server = new GridServer<OrderDetail>(repository.GetForOrder(orderId), new QueryCollection(query),
                    true, "orderDetailssGrid" + keys[0].ToString(), columns)
                        .Sortable()
                        .WithPaging(10)
                        .Filterable()
                        .WithMultipleFilters();

                // return items to displays
                var items = server.ItemsToDisplay;
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
                var repository = new OrdersRepository(context);
                await repository.Insert(item);
                repository.Save();
            }
        }

        public async Task Update(Order item)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                var repository = new OrdersRepository(context);
                await repository.Update(item);
                repository.Save();
            }
        }

        public async Task Delete(params object[] keys)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                var order = Get(keys);
                var repository = new OrdersRepository(context);
                await repository.Delete(order);
                repository.Save();
            }
        }
    }

    public interface IOrderService : ICrudDataService<Order>
    {
        ItemsDTO<Order> GetOrdersGridRows(Action<IGridColumnCollection<Order>> columns, QueryDictionary<StringValues> query);
        ItemsDTO<Order> GetOrdersGridRows(QueryDictionary<StringValues> query);
        ItemsDTO<OrderDetail> GetOrderDetailsGridRows(Action<IGridColumnCollection<OrderDetail>> columns,
            object[] keys, QueryDictionary<StringValues> query);
        Task<Order> GetOrder(int OrderId);
        Task UpdateAndSave(Order order);
    }
}
