using GridBlazor;
using GridBlazorClientSide.Client.Services;
using GridBlazorClientSide.Shared.Models;
using GridShared;
using GridShared.Style;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace GridBlazorClientSide.Client
{
    public class Program
    {
        // used in this sample to get style classes out of GridBlazer
        // not necessary to implement  in normal projects
        public static HtmlClass HtmlClass;

        private readonly static string[] _button_small = { "btn-sm", "btn-sm", "btn-sm", "btn-small", "" };
        public static string ButtonSmall;

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<ICrudDataService<Order>, OrderService>();
            builder.Services.AddScoped<IOrderGridInMemoryService, OrderGridInMemoryService>();
            builder.Services.AddScoped<ICrudDataService<OrderDetail>, OrderDetailService>();
            builder.Services.AddScoped<ICrudDataService<Customer>, CustomerService>();
            builder.Services.AddScoped<ICrudDataService<Employee>, EmployeeService>();
            builder.Services.AddScoped<ICrudFileService<Employee>, EmployeeFileService>();
            builder.Services.AddScoped<ICrudDataService<Truck>, TruckService>();
            builder.Services.AddLocalization();

            var host = builder.Build();

            var jsInterop = host.Services.GetRequiredService<IJSRuntime>();

            var styleStr = await jsInterop.InvokeAsync<string>("blazorStyle.get");
            int styleInt;
            if (string.IsNullOrEmpty(styleStr))
                styleInt = 1;
            else
                int.TryParse(styleStr, out styleInt);
            var style = (CssFramework)styleInt;
            builder.Services.AddGridBlazor(x => x.Style = style);

            // used in this sample to get style classes out of GridBlazer
            // not necessary to implement in normal projects
            HtmlClass = new HtmlClass(style);
            ButtonSmall = _button_small[(int)style];

            var culture = await jsInterop.InvokeAsync<string>("blazorCulture.get");
            if (culture != null)
            {
                var cultureInfo = new CultureInfo(culture);
                CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            }

            await builder.Build().RunAsync();
        }
    }
}
