﻿using BlazorStrap;
using GridBlazorClientSide.Client.Services;
using GridBlazorClientSide.Shared.Models;
using GridShared;
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
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<ICrudDataService<Order>, OrderService>();
            builder.Services.AddScoped<IOrderGridInMemoryService, OrderGridInMemoryService>();
            builder.Services.AddScoped<ICrudDataService<OrderDetail>, OrderDetailService>();
            builder.Services.AddLocalization();

            builder.Services.AddBootstrapCss();

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
