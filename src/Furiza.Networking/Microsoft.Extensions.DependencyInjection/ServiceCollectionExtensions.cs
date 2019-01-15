using Furiza.Networking;
using Furiza.Networking.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFurizaNetworking(this IServiceCollection services)
        {
            services.AddScoped<IHttpClientFactory, HttpClientBasedOnUserPrincipalFactory>();

            return services;
        }
    }
}