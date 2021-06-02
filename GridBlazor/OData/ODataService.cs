using GridShared.OData;
using GridShared.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace GridBlazor.OData
{
    public class ODataService<T> : ICrudODataService<T>
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;
        private readonly CGrid<T> _grid;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions().AddOdataSupport();

        public ODataService(HttpClient httpClient, string url, CGrid<T> grid)
        {
            _httpClient = httpClient;
            _url = url;
            _grid = grid;
        }

        public async Task<T> Get(params object[] keys)
        {
            string url = GetUrl(_grid, _url, keys);
            return await _httpClient.GetFromJsonAsync<T>(url,_jsonOptions);
        }

        public async Task<T> Add(T item)
        {
            
            var response = await _httpClient.PostAsJsonAsync<T>(_url, item, _jsonOptions);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);     
            }
            else
            {
                throw new GridException("ODATA-01", "Error creating the order");
            }
        }

        public async Task Insert(T item)
        {
            await Add(item);
        }

        public async Task Update(T item)
        {
            var keys = _grid.GetPrimaryKeyValues(item);
            string url = GetUrl(_grid, _url, keys);

            var response = await _httpClient.PutAsJsonAsync<T>(url, item, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                throw new GridException("ODATA-02", "Error updating the order");
            }
        }

        public async Task Delete(params object[] keys)
        {
            string url = GetUrl(_grid, _url, keys);
            var response = await _httpClient.DeleteAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new GridException("ODATA-03", "Error deleting the order");
            }
        }

        public static string GetUrl(CGrid<T> grid, string url, params object[] keys)
        {
            var keyNames = grid.GetPrimaryKeys();

            string expandParameters = grid.CurrentExpandODataProcessor.Process();
            if (url.Contains("?"))
                expandParameters = "&" + expandParameters;
            else
                expandParameters = "?" + expandParameters;

            if (keyNames.Length != keys.Length || keys.Length == 1)
                return url + "(" + string.Join(",", keys.Select(x => x.GetType() == typeof(string) ? "'" + x + "'" : x.ToString())) + ")" + expandParameters;
            else
            {
                var keysUrl = new List<string>(); ;
                for (int i = 0; i < keys.Length; i++)
                {
                    keysUrl.Add(keyNames[i] + "=" + (keys[i].GetType() == typeof(string) ? "'" + keys[i] + "'" : keys[i].ToString()));
                }
                return url + "(" + string.Join(",", keysUrl.Select(x => x.ToString())) + ")" + expandParameters;
            }
        }
    }
}
