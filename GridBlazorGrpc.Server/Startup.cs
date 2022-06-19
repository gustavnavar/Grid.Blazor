using GridBlazorGrpc.Server.Models;
using GridBlazorGrpc.Server.Services;
using GridBlazorGrpc.Shared.Services;
using GridShared.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtoBuf.Grpc.Server;
using System;
using System.IO.Compression;

namespace GridBlazorGrpc.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var dbProviderText = Configuration.GetValue<string>("DbProvider");
            if (Enum.TryParse<DbProvider>(dbProviderText, out var dbProvider))
                SharedDbContextUtils.DbProvider = dbProvider;

            services.AddDbContext<NorthwindDbContext>(options =>
            {
                options.UseGridBlazorDatabase();
            });

            services.AddCodeFirstGrpc(options =>
            {
                options.ResponseCompressionLevel = CompressionLevel.Optimal;
            });

            services.AddScoped<ICustomerService, CustomerServerService>();
            services.AddScoped<IEmployeeService, EmployeeServerService>();
            services.AddScoped<IOrderDetailService, OrderDetailServerService>();
            services.AddScoped<IOrderService, OrderServerService>();
            services.AddScoped<IGridService, GridServerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ICustomerService>();
                endpoints.MapGrpcService<IEmployeeService>();
                endpoints.MapGrpcService<IOrderDetailService>();
                endpoints.MapGrpcService<IOrderService>();
                endpoints.MapGrpcService<IGridService>();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
