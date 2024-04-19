using GridBlazor;
using GridBlazorJava.Models;
using GridShared.Utility;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GridBlazorJava.Services
{
    public class EmployeeFileService : ICrudFileService<Employee>
    {
        private readonly int _maxAllowedSize = 5000000;
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;

        public EmployeeFileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUri = "http://localhost:8080/";
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
                    // add OLE header to file data byte array
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

                        // convert byte array to url scaped base64
                        string base64Str = Convert.ToBase64String(ba);
                        base64Str = base64Str.Replace('+', '.');
                        base64Str = base64Str.Replace('/', '_');
                        base64Str = base64Str.Replace('=', '-');

                        item.Base64String = base64Str;
                    }

                    var response = await _httpClient.PostAsJsonAsync(_baseUri + $"api/SampleData/SetEmployeePhoto", item);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new GridException("EMPSRV-04", "Error updating the employee");
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
}
