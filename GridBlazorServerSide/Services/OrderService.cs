using GridBlazorServerSide.Data;
using GridBlazorServerSide.Models;
using GridMvc.Server;
using GridShared;
using GridShared.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;

namespace GridBlazorServerSide.Services
{
    public class OrderService : IOrderService
    {

        private readonly NorthwindDbContext _context;

        public OrderService(NorthwindDbContext context)
        {
            _context = context;
        }

        public ItemsDTO<Order> GetOrdersGridRows(Action<IGridColumnCollection<Order>> columns,
            QueryDictionary<StringValues> query)
        {
            var repository = new OrdersRepository(_context);
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

        public ItemsDTO<Order> GetOrdersGridRows(QueryDictionary<StringValues> query)
        {
            var repository = new OrdersRepository(_context);
            var server = new GridServer<Order>(repository.GetAll(), new QueryCollection(query),
                true, "ordersGrid", null).AutoGenerateColumns();

            // return items to displays
            return server.ItemsToDisplay;
        }

        public ItemsDTO<OrderDetail> GetOrderDetailsGridRows(Action<IGridColumnCollection<OrderDetail>> columns,
            object[] keys, QueryDictionary<StringValues> query)
        {
            var repository = new OrderDetailsRepository(_context);
            var server = new GridServer<OrderDetail>(repository.GetForOrder((int)keys[0]), new QueryCollection(query),
                true, "orderDetailssGrid" + keys[0].ToString(), columns)
                    .Sortable()
                    .WithPaging(10)
                    .Filterable()
                    .WithMultipleFilters();

            // return items to displays
            var items = server.ItemsToDisplay;
            return items;
        }

        public Order GetOrder(int OrderId)
        {
            var repository = new OrdersRepository(_context);
            return repository.GetById(OrderId);
        }

        public void UpdateAndSave(Order order)
        {
            var repository = new OrdersRepository(_context);
            repository.Update(order);
            repository.Save();
        }
    }

    public interface IOrderService
    {
        ItemsDTO<Order> GetOrdersGridRows(Action<IGridColumnCollection<Order>> columns, QueryDictionary<StringValues> query);
        ItemsDTO<Order> GetOrdersGridRows(QueryDictionary<StringValues> query);
        ItemsDTO<OrderDetail> GetOrderDetailsGridRows(Action<IGridColumnCollection<OrderDetail>> columns,
            object[] keys, QueryDictionary<StringValues> query);
        Order GetOrder(int OrderId);
        void UpdateAndSave(Order order);
    }
}
