using GridBlazorClientSide.Client.Services;
using GridBlazorClientSide.Shared.Models;
using GridShared;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace GridBlazorClientSide.Client
{
    public class Program
    {
        public static string Culture //= "fr-FR";
                                     = "en-US";

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddScoped<ICrudDataService<Order>, OrderService>();
            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }
    }
}
