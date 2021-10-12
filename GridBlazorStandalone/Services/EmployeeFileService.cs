using Agno.BlazorInputFile;
using GridBlazor;
using GridBlazorStandalone.Models;
using GridShared.Utility;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazorStandalone.Services
{
    public class EmployeeFileService : IEmployeeFileService
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeFileService(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task InsertFiles(Employee item, IQueryDictionary<IFileListEntry[]> files)
        {
            await UpdateFiles(item, files);
        }

        public async Task<Employee> UpdateFiles(Employee item, IQueryDictionary<IFileListEntry[]> files)
        {
            if (files.Count > 0)
            {
                var file = files.FirstOrDefault();
                if (file.Value.Length > 0)
                {
                    // add OLE header to file data byte array
                    var ms = new MemoryStream();
                    await file.Value[0].Data.CopyToAsync(ms);
                    byte[] ba = new byte[ms.Length + 78];
                    for (int i = 0; i < 78; i++)
                    {
                        ba[i] = 0;
                    }
                    Array.Copy(ms.ToArray(), 0, ba, 78, ms.Length);
                    item.Photo = ba;

                    await _employeeService.Update(item);
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
    { }
}
