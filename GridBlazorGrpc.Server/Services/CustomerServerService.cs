using GridBlazorGrpc.Server.Models;
using GridBlazorGrpc.Shared.Models;
using GridBlazorGrpc.Shared.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GridBlazorGrpc.Server.Services
{
    public class CustomerServerService : ICustomerService
    {
        private readonly NorthwindDbContext _context;

        public CustomerServerService(NorthwindDbContext context)
        {
            _context = context;
        }

        public async ValueTask<IEnumerable<Customer>> GetAll()
        {
            var repository = new CustomersRepository(_context);
            var customers = await repository.GetAll().ToListAsync();
            return customers;
        }

        public async ValueTask<Response> Create(Customer customer)
        {
            if (customer == null)
            {
                new Response(false);
            }

            var repository = new CustomersRepository(_context);
            try
            {
                await repository.Insert(customer);
                repository.Save();

                return new Response(false, customer.CustomerID);
            }
            catch (Exception e)
            {
                return new Response(e);
            }
        }

        public async ValueTask<Customer> Get(Customer customer)
        {
            var repository = new CustomersRepository(_context);
            return await repository.GetById(customer.CustomerID);
        }

        public async ValueTask<Response> Update(Customer customer)
        {
            if (customer == null)
            {
                return new Response(false);
            }

            var repository = new CustomersRepository(_context);
            try
            {
                await repository.Update(customer);
                repository.Save();

                return new Response(true);
            }
            catch (Exception e)
            {
                return new Response(e);
            }
        }

        public async ValueTask<Response> Delete(Customer customer)
        {
            var repository = new CustomersRepository(_context);
            Customer attachedItem = await repository.GetById(customer.CustomerID);

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
