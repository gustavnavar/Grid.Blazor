using GridBlazor;
using GridBlazorServerSide.Data;
using GridBlazorServerSide.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Globalization;
using GridShared.Data;
using GridShared.Style;

namespace GridBlazorServerSide
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
            services.AddDbContext<NorthwindDbContext>(options =>
            {
                options.UseGridBlazorDatabase();
                //options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.QueryClientEvaluationWarning));
            });

            services.AddGridBlazor(x => x.Style = CssFramework.Bootstrap_4);

            services.AddControllers();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            //.AddCircuitOptions(options => { options.DetailedErrors = true; });
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderDetailService, OrderDetailService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IShipperService, ShipperService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ITruckService, TruckService>();
            services.AddScoped<IEmployeeFileService, EmployeeFileService>();

            services.Configure<RequestLocalizationOptions>(
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
                            new CultureInfo("nl-NL"),
                            new CultureInfo("tr-TR"),
                            new CultureInfo("cs-CZ"),
                            new CultureInfo("sl-SI"),
                            new CultureInfo("se-SE"),
                            new CultureInfo("sr-Latn-RS"),
                            new CultureInfo("sr-Cyrl-RS"),
                            new CultureInfo("sr-Latn-BA"),
                            new CultureInfo("sr-Cyrl-BA"),
                            new CultureInfo("hr-HR"),
                            new CultureInfo("fa-IR"),
                            new CultureInfo("ca-ES"),
                            new CultureInfo("gl-ES"),
                            new CultureInfo("eu-ES"),
                            new CultureInfo("pt-BR"),
                            new CultureInfo("bg-BG"),
                            new CultureInfo("uk-UA"),
                            new CultureInfo("ar-EG"),
                            new CultureInfo("da-DK"),
                            new CultureInfo("ja-JP"),
                            new CultureInfo("zh-Hans-CN"),
                            new CultureInfo("zh-Hant-CN")
                        };

                    options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
