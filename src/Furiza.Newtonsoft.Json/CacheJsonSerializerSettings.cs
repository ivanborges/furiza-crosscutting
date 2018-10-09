namespace Newtonsoft.Json
{
    public class CacheJsonSerializerSettings : GeneralJsonSerializerSettings
    {
        public CacheJsonSerializerSettings() : base()
        {
            TypeNameHandling = TypeNameHandling.Auto;
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full;
        }
    }
}