using GridBlazorClientSide.Client.Services;
using GridBlazorClientSide.Shared.Models;
using GridShared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace GridBlazorClientSide.Client
{
    public class Program
    {
        public static string Culture = "en-US";

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddBaseAddressHttpClient();
            builder.Services.AddScoped<ICrudDataService<Order>, OrderService>();
            builder.Services.AddScoped<IOrderGridInMemoryService, OrderGridInMemoryService>();
            builder.Services.AddScoped<ICrudDataService<OrderDetail>, OrderDetailService>();
            builder.Services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var supportedCultures = new List<CultureInfo>
                        {
                            new CultureInfo("en-US"),
                            new CultureInfo("de-DE"),
                            new CultureInfo("it-IT"),
                            new CultureInfo("es-ES"),
                            new CultureInfo("fr-FR"),
                            new CultureInfo("ru-RU"),
                            new CultureInfo("nb-NO"),
                            new CultureInfo("tr-TR"),
                            new CultureInfo("cs-CZ"),
                            new CultureInfo("sl-SI")
                        };

                    options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                });
            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }
    }
}
