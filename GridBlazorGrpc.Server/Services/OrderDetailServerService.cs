using GridBlazorGrpc.Server.Models;
using GridBlazorGrpc.Shared.Models;
using GridBlazorGrpc.Shared.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GridBlazorGrpc.Server.Services
{
    public class OrderDetailServerService : IOrderDetailService
    {
        private readonly NorthwindDbContext _context;

        public OrderDetailServerService(NorthwindDbContext context)
        {
            _context = context;
        }

        public async ValueTask<IEnumerable<OrderDetail>> GetAll()
        {
            var repository = new OrderDetailsRepository(_context);
            var orders = await repository.GetAll().ToListAsync();
            return orders;
        }

        public async ValueTask<Response> Create(OrderDetail orderDetail)
        {
            if (orderDetail == null)
            {
                return new Response(false);
            }

            var repository = new OrderDetailsRepository(_context);
            try
            {
                await repository.Insert(orderDetail);
                repository.Save();

                return new Response(true);
            }
            catch (Exception e)
            {
                return new Response(e);
            }
        }

        public async ValueTask<OrderDetail> Get(OrderDetail orderDetail)
        {
            var repository = new OrderDetailsRepository(_context);
            return await repository.GetById(new { OrderID = orderDetail.OrderID, ProductID = orderDetail.ProductID });
        }

        public async ValueTask<Response> Update(OrderDetail orderDetail)
        {
            if (orderDetail == null)
            {
                return new Response(false);
            }

            var repository = new OrderDetailsRepository(_context);
            try
            {
                await repository.Update(orderDetail);
                repository.Save();

                return new Response(true);
            }
            catch (Exception e)
            {
                return new Response(e);
            }
        }

        public async ValueTask<Response> Delete(OrderDetail orderDetail)
        {
            var repository = new OrderDetailsRepository(_context);
            OrderDetail attachedItem = await repository.GetById(new { OrderID = orderDetail.OrderID, ProductID = orderDetail.ProductID });

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
