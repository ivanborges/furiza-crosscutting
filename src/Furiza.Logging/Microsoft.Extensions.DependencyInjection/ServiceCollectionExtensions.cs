using Furiza.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Json;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFurizaLogging(this IServiceCollection services, IConfiguration configuration, string applicationName)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var logConfiguration = configuration.TryGet<LogConfiguration>();
            switch (logConfiguration.Tool.Value)
            {
                case LogTool.Serilog:
                    if (logConfiguration.Serilog == null)
                        throw new InvalidOperationException($"Configuration section '{nameof(LogConfiguration)}.{nameof(LogConfiguration.Serilog)}' is missing.");

                    if (logConfiguration.Serilog.MinimumLevel == null)
                        throw new InvalidOperationException($"Configuration section '{nameof(LogConfiguration)}.{nameof(LogConfiguration.Serilog)}.{nameof(LogConfiguration.Serilog.MinimumLevel)}' is missing.");

                    if (string.IsNullOrWhiteSpace(logConfiguration.Serilog.MinimumLevel.Default))
                        throw new InvalidOperationException($"Attribute '{nameof(LogConfiguration.Serilog.MinimumLevel.Default)}' for configuration section '{nameof(LogConfiguration)}.{nameof(LogConfiguration.Serilog)}.{nameof(LogConfiguration.Serilog.MinimumLevel)}' is missing.");
                    
                    services.AddLogging(c =>
                    {
                        c.ClearProviders();

                        Log.Logger = new LoggerConfiguration()
                            .Enrich.FromLogContext()
                            .ReadFrom.Configuration(configuration.GetSection(nameof(LogConfiguration)))
                            .WriteTo.Async(a => a.Console(new JsonFormatter(renderMessage: true)))
                            .WriteTo.Async(a => a.RollingFile($"logs/{applicationName}-log.txt"))
                            .CreateLogger();

                        c.AddSerilog();
                    });
                    break;
                default:
                    throw new InvalidOperationException($"Logging provider '{logConfiguration.Tool.Value}' unavailable.");
            }           

            return services;
        }
    }
}