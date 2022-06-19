using GridBlazorGrpc.Server.Models;
using GridBlazorGrpc.Shared.Models;
using GridBlazorGrpc.Shared.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GridBlazorGrpc.Server.Services
{
    public class EmployeeServerService : IEmployeeService
    {
        private readonly NorthwindDbContext _context;

        public EmployeeServerService(NorthwindDbContext context)
        {
            _context = context;
        }

        public async ValueTask<IEnumerable<Employee>> GetAll()
        {
            var repository = new EmployeeRepository(_context);
            var employees = await repository.GetAll().ToListAsync();
            return employees;
        }

        public async ValueTask<Response> Create(Employee employee)
        {
            if (employee == null)
            {
                return new Response(false);
            }

            var repository = new EmployeeRepository(_context);
            try
            {
                await repository.Insert(employee);
                repository.Save();

                return new Response(true, employee.EmployeeID);
            }
            catch (Exception e)
            {
                return new Response(e);
            }
        }

        public async ValueTask<Employee> Get(Employee employee)
        {
            var repository = new EmployeeRepository(_context);
            return await repository.GetById(employee.EmployeeID);
        }

        public async ValueTask<Response> Update(Employee employee)
        {
            if (employee == null)
            {
                return new Response(false);
            }

            var repository = new EmployeeRepository(_context);
            try
            {
                await repository.Update(employee);
                repository.Save();

                return new Response(true);
            }
            catch (Exception e)
            {
                return new Response(e);
            }
        }

        public async ValueTask<Response> Delete(Employee employee)
        {
            var repository = new EmployeeRepository(_context);
            Employee attachedItem = await repository.GetById(employee.EmployeeID);

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
