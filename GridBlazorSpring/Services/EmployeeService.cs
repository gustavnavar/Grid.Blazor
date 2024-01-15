using GridBlazorSpring.Models;
using GridShared;
using GridShared.Utility;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GridBlazorSpring.Services
{
    public class EmployeeService : ICrudDataService<Employee>
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;

        public EmployeeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUri = "http://localhost:8080/";
        }

        public async Task<Employee> Get(params object[] keys)
        {
            int employeeId;
            int.TryParse(keys[0].ToString(), out employeeId);
            return await _httpClient.GetFromJsonAsync<Employee>(_baseUri + $"api/Employee/{employeeId}");
        }

        public async Task Insert(Employee item)
        {
            item.Base64String = null;
            var response = await _httpClient.PostAsJsonAsync(_baseUri + $"api/Employee", item);
            if (response.IsSuccessStatusCode)
            {
                item.EmployeeID = Convert.ToInt32(await response.Content.ReadAsStringAsync());              
            }
            else
            {
                throw new GridException("EMPSRV-01", "Error creating the employee");
            }
        }

        public async Task Update(Employee item)
        {
            item.Base64String = null;
            var response = await _httpClient.PutAsJsonAsync(_baseUri + $"api/Employee/{item.EmployeeID}", item);
            if (!response.IsSuccessStatusCode)
            {
                throw new GridException("EMPSRV-02", "Error updating the employee");
            }
        }

        public async Task Delete(params object[] keys)
        {
            int employeeId;
            int.TryParse(keys[0].ToString(), out employeeId);
            var response = await _httpClient.DeleteAsync(_baseUri + $"api/Employee/{employeeId}");
            if (!response.IsSuccessStatusCode)
            {
                throw new GridException("EMPSRV-03", "Error deleting the employee");
            }
        }
    }
}
