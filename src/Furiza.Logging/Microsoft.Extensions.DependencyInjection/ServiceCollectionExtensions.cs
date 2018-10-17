using Furiza.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Linq;

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
                    ValidateSerilogStructure(logConfiguration);

                    services.AddLogging(c =>
                    {
                        c.ClearProviders();

                        Log.Logger = new LoggerConfiguration()
                            .ReadFrom.Configuration(configuration.GetSection(nameof(LogConfiguration)))
                            .CreateLogger();

                        c.AddSerilog();
                    });
                    break;
                default:
                    throw new InvalidOperationException($"Logging provider '{logConfiguration.Tool.Value}' unavailable.");
            }           

            return services;
        }

        private static void ValidateSerilogStructure(LogConfiguration logConfiguration)
        {
            if (logConfiguration.Serilog == null)
                throw new InvalidOperationException($"Configuration section '{nameof(LogConfiguration)}.{nameof(LogConfiguration.Serilog)}' is missing.");

            if (logConfiguration.Serilog.Using == null)
                throw new InvalidOperationException($"Attribute '{nameof(LogConfiguration.Serilog.Using)}' for configuration section '{nameof(LogConfiguration)}.{nameof(LogConfiguration.Serilog)}' is missing.");

            if (!logConfiguration.Serilog.Using.Any())
                throw new InvalidOperationException($"Attribute '{nameof(LogConfiguration.Serilog.Using)}' for configuration section '{nameof(LogConfiguration)}.{nameof(LogConfiguration.Serilog)}' must have at least 1 serilog package reference.");

            if (logConfiguration.Serilog.MinimumLevel == null)
                throw new InvalidOperationException($"Configuration section '{nameof(LogConfiguration)}.{nameof(LogConfiguration.Serilog)}.{nameof(LogConfiguration.Serilog.MinimumLevel)}' is missing.");

            if (string.IsNullOrWhiteSpace(logConfiguration.Serilog.MinimumLevel.Default))
                throw new InvalidOperationException($"Attribute '{nameof(LogConfiguration.Serilog.MinimumLevel.Default)}' for configuration section '{nameof(LogConfiguration)}.{nameof(LogConfiguration.Serilog)}.{nameof(LogConfiguration.Serilog.MinimumLevel)}' is missing.");

            if (logConfiguration.Serilog.WriteTo == null)
                throw new InvalidOperationException($"Configuration section '{nameof(LogConfiguration)}.{nameof(LogConfiguration.Serilog)}.{nameof(LogConfiguration.Serilog.WriteTo)}' is missing.");

            if (!logConfiguration.Serilog.WriteTo.Any())
                throw new InvalidOperationException($"Configuration section '{nameof(LogConfiguration)}.{nameof(LogConfiguration.Serilog)}.{nameof(LogConfiguration.Serilog.WriteTo)}' must have at least 1 logging output.");
        }
    }
}