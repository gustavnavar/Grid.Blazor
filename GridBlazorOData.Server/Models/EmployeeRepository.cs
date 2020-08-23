using GridBlazorOData.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazorOData.Server.Models
{
    public class EmployeeRepository : SqlRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(NorthwindDbContext context)
            : base(context)
        {
        }

        public override IQueryable<Employee> GetAll()
        {
            return EfDbSet;
        }

        public override async Task<Employee> GetById(object id)
        {
            return await GetAll().SingleOrDefaultAsync(c => c.EmployeeID == (int)id);
        }


        public async Task Insert(Employee employee)
        {
            await EfDbSet.AddAsync(employee);
        }

        public async Task Update(Employee employee)
        {
            var entry = Context.Entry(employee);
            if (entry.State == EntityState.Detached)
            {
                var attachedEmployee = await GetById(employee.EmployeeID);
                if (attachedEmployee != null)
                {
                    var photo = attachedEmployee.Photo;
                    Context.Entry(attachedEmployee).CurrentValues.SetValues(employee);
                    attachedEmployee.Photo = photo;
                }
                else
                {
                    entry.State = EntityState.Modified;
                }
            }
        }

        public async Task UpdatePhoto(int id, string photo)
        {
            var employee = await GetById(id);
            if (employee != null)
            {
                // convert url scaped base64 to byte array
                photo = photo.Replace('.', '+');
                photo = photo.Replace('_', '/');
                photo = photo.Replace('-', '=');
                employee.Photo = Convert.FromBase64String(photo);
            }
        }

        public void Delete(Employee employee)
        {
            EfDbSet.Remove(employee);
        }

        public void Save()
        {
            Context.SaveChanges();
        }
    }

    public interface IEmployeeRepository
    {
        Task Insert(Employee employee);
        Task Update(Employee employee);
        Task UpdatePhoto(int id, string photo);
        void Delete(Employee employee);
        void Save();
    }
}