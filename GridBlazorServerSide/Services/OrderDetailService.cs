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
    public class OrderDetailService : IOrderDetailService
    {
        private readonly DbContextOptions<NorthwindDbContext> _options;

        public OrderDetailService(DbContextOptions<NorthwindDbContext> options)
        {
            _options = options;
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

        public async Task<OrderDetail> Get(params object[] keys)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                int orderId;
                int productId;
                int.TryParse(keys[0].ToString(), out orderId);
                int.TryParse(keys[1].ToString(), out productId);
                var repository = new OrderDetailsRepository(context);
                return await repository.GetById(new { OrderID = orderId, ProductID = productId });
            }
        }

        public async Task Insert(OrderDetail item)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                try
                {
                    var repository = new OrderDetailsRepository(context);
                    await repository.Insert(item);
                    repository.Save();
                }
                catch (Exception e)
                {
                    throw new GridException("DETSRV-01", e);
                }
            }
        }

        public async Task Update(OrderDetail item)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                try
                {
                    var repository = new OrderDetailsRepository(context);
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
                    var repository = new OrderDetailsRepository(context);
                    repository.Delete(order);
                    repository.Save();
                }
                catch (Exception)
                {
                    throw new GridException("Error deleting the order detail");
                }
            }
        }
    }

    public interface IOrderDetailService : ICrudDataService<OrderDetail>
    {
        ItemsDTO<OrderDetail> GetOrderDetailsGridRows(Action<IGridColumnCollection<OrderDetail>> columns,
            object[] keys, QueryDictionary<StringValues> query);
    }
}
