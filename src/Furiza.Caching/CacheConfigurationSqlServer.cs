namespace Furiza.Caching
{
    public class CacheConfigurationSqlServer
    {
        public string ConnectionString { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
    }
}