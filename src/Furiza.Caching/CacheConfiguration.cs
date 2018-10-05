using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Furiza.Caching
{
    public class CacheConfiguration
    {
        [Required]
        public CacheTool? Tool { get; set; }

        [Required]
        public bool? Enable { get; set; }

        [Required]
        public int? DefaultExpiration { get; set; }

        public IEnumerable<CacheConfigurationExpirationEntry> CustomExpirations { get; set; }

        public CacheConfigurationSqlServer SqlServer { get; set; }

        public CacheConfigurationRedis Redis { get; set; }
    }
}