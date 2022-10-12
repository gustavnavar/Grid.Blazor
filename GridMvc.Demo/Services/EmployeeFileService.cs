using GridBlazor;
using GridMvc.Demo.Models;
using GridShared.Utility;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GridMvc.Demo.Services
{
    public class EmployeeFileService : IEmployeeFileService
    {
        private readonly int _maxAllowedSize = 5000000;
        private readonly DbContextOptions<NorthwindDbContext> _options;

        public EmployeeFileService(DbContextOptions<NorthwindDbContext> options)
        {
            _options = options;
        }

        public async Task InsertFiles(Employee item, IQueryDictionary<IBrowserFile[]> files)
        {
            await UpdateFiles(item, files);
        }

        public async Task<Employee> UpdateFiles(Employee item, IQueryDictionary<IBrowserFile[]> files)
        {
            if (files.Count > 0)
            {
                var file = files.FirstOrDefault();
                if (file.Value.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    using (var stream = file.Value[0].OpenReadStream(_maxAllowedSize))
                    {
                        await stream.CopyToAsync(ms);
                        byte[] ba = new byte[ms.Length + 78];
                        for (int i = 0; i < 78; i++)
                        {
                            ba[i] = 0;
                        }
                        Array.Copy(ms.ToArray(), 0, ba, 78, ms.Length);
                        item.Photo = ba;
                    }

                    using (var context = new NorthwindDbContext(_options))
                    {
                        try
                        {
                            var repository = new EmployeeRepository(context);
                            await repository.Update(item);
                            repository.Save();
                        }
                        catch (Exception e)
                        {
                            throw new GridException(e);
                        }
                    }
                }
            }
            return item;
        }

        public async Task DeleteFiles(params object[] keys)
        {
            // do nothing, deleting the database record is enough
            await Task.CompletedTask;
        }
    }

    public interface IEmployeeFileService : ICrudFileService<Employee>
    {
    }
}
