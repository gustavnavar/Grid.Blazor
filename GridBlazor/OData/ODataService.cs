using GridShared;
using GridShared.Utility;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace GridBlazor.OData
{
    public class ODataService<T> : ICrudDataService<T>
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;
        private readonly ICGrid _grid;

        public ODataService(HttpClient httpClient, string url, ICGrid grid)
        {
            _httpClient = httpClient;
            _url = url;
            _grid = grid;
        }

        public async Task<T> Get(params object[] keys)
        {
            string url = GetUrl(_url, keys);
            return await _httpClient.GetFromJsonAsync<T>(url);
        }

        public async Task Insert(T item)
        {
            var jsonOptions = new JsonSerializerOptions().AddOdataSupport();
            var response = await _httpClient.PostAsJsonAsync<T>(_url, item, jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                throw new GridException("ODATA-01", "Error creating the order");
            }
        }

        public async Task Update(T item)
        {
            var keys = _grid.GetPrimaryKeyValues(item);
            string url = GetUrl(_url, keys);

            var jsonOptions = new JsonSerializerOptions().AddOdataSupport();
            var response = await _httpClient.PutAsJsonAsync<T>(url, item, jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                throw new GridException("ODATA-02", "Error updating the order");
            }
        }

        public async Task Delete(params object[] keys)
        {
            string url = GetUrl(_url, keys);
            var response = await _httpClient.DeleteAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new GridException("ODATA-03", "Error deleting the order");
            }
        }

        public static string GetUrl(string url, params object[] keys)
        {
            return url + "(" + string.Join(",", keys.Select(x => x.ToString())) + ")";
        }
    }
}
