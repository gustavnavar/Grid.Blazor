using GridComponent.Demo.Components;
using GridComponent.Demo.Models;
using GridComponent.Demo.Services;
using GridMvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GridComponent.Demo
{
    public class Startup
    {
        public static string ConnectionString = "Server=.\\SQLEXPRESS;Database=NorthWind;Trusted_Connection=True;MultipleActiveResultSets=true";

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<NorthwindDbContext>(options =>
                options.UseSqlServer(ConnectionString));

            services.AddMvc()
                .AddNewtonsoftJson();

            services.AddRazorComponents();

            // to ennable use of gridmvc.css
            services.AddGridMvc();

            services.AddSingleton<WeatherForecastService>();
            services.AddSingleton<OrderService>();
            services.AddSingleton<CustomerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting(routes =>
            {
                routes.MapRazorPages();
                routes.MapComponentHub<App>("app");
            });
        }
    }
}
