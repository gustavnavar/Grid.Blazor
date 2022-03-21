using GridMvc.Demo.Models;
using GridMvc.Server;
using GridShared;
using GridShared.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace GridMvc.Demo.Services
{
    public class OrderService : IOrderService
    {
        private readonly DbContextOptions<NorthwindDbContext> _options;

        public OrderService(DbContextOptions<NorthwindDbContext> options)
        {
            _options = options;
        }

        public async Task<ItemsDTO<Order>> GetOrdersGridRowsAsync(Action<IGridColumnCollection<Order>> columns,
            QueryDictionary<StringValues> query)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                var repository = new OrdersRepository(context);
                var server = new GridServer<Order>(repository.GetAll().Include(r => r.OrderDetails), new QueryCollection(query),
                    true, "ordersGrid", columns)
                        .Sortable()
                        .WithPaging(10)
                        .Filterable()
                        .WithMultipleFilters()
                        .Groupable(true)
                        .Searchable(true, false, false)
                        .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

                // return items to displays
                var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
                return items;
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
                var order = await Get(keys);
                var repository = new OrdersRepository(context);
                repository.Delete(order);
                repository.Save();
            }
        }
    }

    public interface IOrderService : ICrudDataService<Order>
    {
        Task<ItemsDTO<Order>> GetOrdersGridRowsAsync(Action<IGridColumnCollection<Order>> columns, QueryDictionary<StringValues> query);
    }
}
