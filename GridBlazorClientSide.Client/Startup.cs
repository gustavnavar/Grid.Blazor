using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace GridBlazorClientSide.Client
{
    public class Startup
    {
        public static string Culture //= "fr-FR";
                                     = "en-US";

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
