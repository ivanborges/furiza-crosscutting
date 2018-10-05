using Furiza.Caching;
using Furiza.Caching.Abstractions;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFurizaCaching(this IServiceCollection services, CacheConfiguration cacheConfiguration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton(cacheConfiguration ?? throw new ArgumentNullException(nameof(cacheConfiguration)));
            
            switch (cacheConfiguration.Tool.Value)
            {
                case CacheTool.Memory:
                    services.AddDistributedMemoryCache();
                    break;
                case CacheTool.SqlServer:
                    if (cacheConfiguration.SqlServer == null)
                        throw new InvalidOperationException($"Configuration section '{nameof(CacheConfiguration)}.{nameof(CacheConfiguration.SqlServer)}' is missing.");

                    if (cacheConfiguration.SqlServer
                        .GetType()
                        .GetProperties()
                        .Where(pi => pi.GetValue(cacheConfiguration.SqlServer) is string)
                        .Select(pi => (string)pi.GetValue(cacheConfiguration.SqlServer))
                        .Any(value => string.IsNullOrWhiteSpace(value)))
                        throw new InvalidOperationException($"Some attribute for configuration section '{nameof(CacheConfiguration)}.{nameof(CacheConfiguration.SqlServer)}' is missing.");

                    services.AddDistributedSqlServerCache(options =>
                    {
                        options.ConnectionString = cacheConfiguration.SqlServer.ConnectionString;
                        options.SchemaName = cacheConfiguration.SqlServer.SchemaName;
                        options.TableName = cacheConfiguration.SqlServer.TableName;
                    });
                    break;
                case CacheTool.Redis:
                    if (cacheConfiguration.Redis == null)
                        throw new InvalidOperationException($"Configuration section '{nameof(CacheConfiguration)}.{nameof(CacheConfiguration.Redis)}' is missing.");

                    if (cacheConfiguration.Redis
                        .GetType()
                        .GetProperties()
                        .Where(pi => pi.GetValue(cacheConfiguration.Redis) is string)
                        .Select(pi => (string)pi.GetValue(cacheConfiguration.Redis))
                        .Any(value => string.IsNullOrWhiteSpace(value)))
                        throw new InvalidOperationException($"Some attribute for configuration section '{nameof(CacheConfiguration)}.{nameof(CacheConfiguration.Redis)}' is missing.");

                    services.AddDistributedRedisCache(options =>
                    {
                        options.InstanceName = cacheConfiguration.Redis.InstanceName;
                        options.Configuration = cacheConfiguration.Redis.Configuration;
                    });
                    break;
            }

            services.AddScoped<ICacheHandler, CacheHandler>();

            return services;
        }
    }
}