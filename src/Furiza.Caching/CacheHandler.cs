using Furiza.Caching.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Furiza.Caching
{
    internal class CacheHandler : ICacheHandler
    {
        private readonly ILogger logger;
        private readonly IDistributedCache distributedCache;
        private readonly CacheConfiguration cacheConfiguration;

        public CacheHandler(ILoggerFactory loggerFactory,
            IDistributedCache distributedCache,
            CacheConfiguration cacheConfiguration)
        {
            logger = loggerFactory?.CreateLogger<CacheHandler>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            this.cacheConfiguration = cacheConfiguration ?? throw new ArgumentNullException(nameof(cacheConfiguration));
        }

        public bool TryGetValue<T>(string key, out T value) where T : class
        {
            if (!cacheConfiguration.Enable.Value)
            {
                value = null;
                return false;
            }

            var fullKey = BuildKey(typeof(T), key);
            var cache = string.Empty;

            try
            {
                cache = distributedCache.GetString(key);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An internal error occurred while trying to access the cache with the key [{fullKey}].");
                value = null;
                return false;
            }

            if (string.IsNullOrWhiteSpace(cache))
            {
                value = null;
                return false;
            }

            try
            {
                value = JsonConvert.DeserializeObject<T>(cache, new CacheJsonSerializerSettings());
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An internal error occurred while trying to deserialize the cache with the key [{fullKey}].");
                value = null;
                return false;
            }
        }

        public async Task SetAsync<T>(string key, T value, IEnumerable<string> namesOfPropertiesToIgnore = null) where T : class
        {
            if (!cacheConfiguration.Enable.Value)
                return;

            var fullKey = BuildKey(typeof(T), key);
            var expiration = GetExpiration(typeof(T));
            var cache = JsonConvert.SerializeObject(value, typeof(T), Formatting.Indented, new CacheJsonSerializerSettings().IgnoreProperties(typeof(T), namesOfPropertiesToIgnore));

            try
            {
                await distributedCache.SetStringAsync(key, cache, new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(expiration)
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An internal error occurred while trying to set the cache with the key [{fullKey}].");
            }
        }

        public async Task RemoveAsync<T>(string key) where T : class
        {
            if (!cacheConfiguration.Enable.Value)
                return;

            var fullKey = BuildKey(typeof(T), key);

            try
            {
                await distributedCache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An internal error occurred while trying to remove the cache with the key [{fullKey}].");
            }
        }
        
        #region [+] Privates
        private string GetAlias(Type t) => $"{t.Namespace}.{t.Name.Split('`')[0]}";

        private string BuildKey(Type t, string key) => $"{GetAlias(t)}.{key.Trim().ToLower()}";

        private int GetExpiration(Type t)
        {
            var customExpiration = cacheConfiguration.CustomExpirations?.SingleOrDefault(e => e.Class.Trim().ToUpper() == GetAlias(t).Trim().ToUpper())?.Time;
            return (customExpiration ?? cacheConfiguration.DefaultExpiration).Value;
        }
        #endregion
    }
}