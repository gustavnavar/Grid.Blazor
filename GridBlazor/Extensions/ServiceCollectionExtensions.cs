using GridShared;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GridBlazor
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGridBlazor(this IServiceCollection serviceCollection, Action<IGridBlazorOptions> options = null)
        {
            serviceCollection.AddSingleton<IGridBlazorService>(x => new GridBlazorService(options));
            return serviceCollection;
        }
    }
}
