using Agno.BlazorInputFile;
using GridBlazor;
using GridBlazorServerSide.Data;
using GridBlazorServerSide.Models;
using GridShared.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazorServerSide.Services
{
    public class EmployeeFileService : IEmployeeFileService
    {
        private readonly DbContextOptions<NorthwindDbContext> _options;

        public EmployeeFileService(DbContextOptions<NorthwindDbContext> options)
        {
            _options = options;
        }

        public async Task InsertFiles(Employee item, IQueryDictionary<IFileListEntry[]> files)
        {
            await UpdateFiles(item, files);
        }

        public async Task UpdateFiles(Employee item, IQueryDictionary<IFileListEntry[]> files)
        {
            if (files.Count > 0)
            {
                var file = files.FirstOrDefault();
                if (file.Value.Length > 0)
                {
                    var ms = new MemoryStream();
                    await file.Value[0].Data.CopyToAsync(ms);
                    byte[] ba = new byte[ms.Length + 78];
                    for (int i = 0; i < 78; i++)
                    {
                        ba[i] = 0;
                    }
                    Array.Copy(ms.ToArray(), 0, ba, 78, ms.Length);
                    item.Photo = ba;

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
