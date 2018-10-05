namespace Newtonsoft.Json
{
    public class CacheJsonSerializerSettings : JsonSerializerSettings
    {
        public CacheJsonSerializerSettings() : base()
        {
            TypeNameHandling = TypeNameHandling.Auto;
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full;
        }
    }
}