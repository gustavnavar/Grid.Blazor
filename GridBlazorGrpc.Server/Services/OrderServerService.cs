using GridBlazorGrpc.Server.Models;
using GridBlazorGrpc.Shared.Models;
using GridBlazorGrpc.Shared.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GridBlazorGrpc.Server.Services
{
    public class OrderServerService : IOrderService
    {
        private readonly NorthwindDbContext _context;

        public OrderServerService(NorthwindDbContext context)
        {
            _context = context;
        }

        public async ValueTask<IEnumerable<Order>> GetAll()
        {
            var repository = new OrdersRepository(_context);
            var orders = await repository.GetAll().ToListAsync();
            return orders;
        }

        public async ValueTask<Response> Create(Order order)
        {
            if (order == null)
            {
                return new Response(false);
            }

            var repository = new OrdersRepository(_context);
            try
            {
                await repository.Insert(order);
                repository.Save();

                return new Response(true, order.OrderID);
            }
            catch (Exception e)
            {
                return new Response(e);
            }
        }

        public async ValueTask<Order> Get(Order order)
        {
            var repository = new OrdersRepository(_context);
            return await repository.GetById(order.OrderID);
        }

        public async ValueTask<Response> Update(Order order)
        {
            if (order == null)
            {
                return new Response(false);
            }

            var repository = new OrdersRepository(_context);
            try
            {
                await repository.Update(order);
                repository.Save();

                return new Response(true);
            }
            catch (Exception e)
            {
                return new Response(e);
            }
        }

        public async ValueTask<Response> Delete(Order order)
        {
            var repository = new OrdersRepository(_context);
            Order attachedItem = await repository.GetById(order.OrderID);

            if (attachedItem == null)
            {
                return new Response(false);
            }

            try
            {
                repository.Delete(attachedItem);
                repository.Save();

                return new Response(true);
            }
            catch (Exception e)
            {
                return new Response(e);
            }
        }
    }
}
