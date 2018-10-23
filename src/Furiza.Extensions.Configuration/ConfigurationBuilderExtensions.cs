using Furiza.Extensions.Configuration;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Adds object configuration provider to configuration builder.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="configurationObject">Any object that carry configuration information.</param>
        /// <returns></returns>
        public static IConfigurationBuilder AddObject<T>(this IConfigurationBuilder builder, T configurationObject)
            where T : class
        {
            return builder.Add(new JsonObjectConfigurationSource(configurationObject));
        }
    }
}