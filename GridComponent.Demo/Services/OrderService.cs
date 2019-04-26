using GridShared;
using GridShared.Utility;
using GridBlazor.Demo.Shared.Models;
using GridComponent.Demo.Models;
using GridMvc.Server;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;

namespace GridComponent.Demo.Services
{
    public class OrderService
    {
        private readonly NorthwindDbContext _context;

        public OrderService()
        {
            var builder = new DbContextOptionsBuilder<NorthwindDbContext>();
            builder.UseSqlServer(Startup.ConnectionString);
            _context = new NorthwindDbContext(builder.Options);
        }

        public ItemsDTO<Order> GetOrdersGridRows(Action<IGridColumnCollection<Order>> columns,
            QueryDictionary<StringValues> query)
        {
            var repository = new OrdersRepository(_context);
            var server = new GridServer<Order>(repository.GetAll(), new QueryCollection(query), 
                true, "ordersGrid", columns, 10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters();
            
            // return items to displays
            return server.ItemsToDisplay;
        }
    }
}
