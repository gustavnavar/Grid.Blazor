using GridBlazor;
using GridBlazorGrpc.Client.Services;
using GridBlazorGrpc.Shared.Models;
using GridShared;
using GridShared.Style;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace GridBlazorGrpc.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<IGridClientService, GridClientService>();
            builder.Services.AddScoped<ICrudDataService<Order>, OrderClientService>();
            builder.Services.AddScoped<IOrderGridInMemoryService, OrderGridInMemoryService>();
            builder.Services.AddScoped<ICrudDataService<OrderDetail>, OrderDetailClientService>();
            builder.Services.AddScoped<ICrudDataService<Customer>, CustomerClientService>();
            builder.Services.AddScoped<ICrudDataService<Employee>, EmployeeClientService>();
            builder.Services.AddScoped<ICrudFileService<Employee>, EmployeeFileService>();
            builder.Services.AddLocalization();

            builder.Services.AddGridBlazor(x => x.Style = CssFramework.Bootstrap_4);

            var host = builder.Build();
            var jsInterop = host.Services.GetRequiredService<IJSRuntime>();
            var result = await jsInterop.InvokeAsync<string>("blazorCulture.get");
            if (result != null)
            {
                var culture = new CultureInfo(result);
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }

            await builder.Build().RunAsync();
        }
    }
}
