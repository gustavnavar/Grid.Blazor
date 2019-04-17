using Microsoft.Extensions.DependencyInjection;

namespace GridMvc
{
    public static class GridMvcServiceCollectionExtensions
    {
        public static void AddGridMvc(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(GridMvcConfigureOptions));
        }
    }
}
