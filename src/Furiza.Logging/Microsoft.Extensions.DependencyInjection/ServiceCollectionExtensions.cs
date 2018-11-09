using Furiza.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFurizaLogging(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var logConfiguration = configuration.TryGet<LogConfiguration>();
            switch (logConfiguration.Tool.Value)
            {
                case LogTool.Serilog:
                    var loggerConfiguration = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .MinimumLevel.Override("System", LogEventLevel.Error)
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                        .MinimumLevel.Override("Furiza", LogEventLevel.Warning)
                        .ReadFrom.Configuration(configuration.GetSection(nameof(LogConfiguration)))
                        .Enrich.WithExceptionDetails();

                    if (logConfiguration.WriteToType != LogWriteTo.Custom)
                    {
                        loggerConfiguration = loggerConfiguration
                            .Enrich.FromLogContext()
                            .Enrich.WithProcessId()
                            .Enrich.WithProcessName()
                            .Enrich.WithThreadId()
                            .Enrich.WithEnvironmentUserName()
                            .Enrich.WithMachineName();

                        if (logConfiguration.WriteToType == LogWriteTo.File)
                            loggerConfiguration = loggerConfiguration.WriteTo.Async(a => a.File($"logs/log-.txt", rollingInterval: RollingInterval.Day));
                        else
                            loggerConfiguration = loggerConfiguration.WriteTo.Async(a => a.Console(new JsonFormatter()));
                    }

                    services.AddLogging(c =>
                    {
                        c.ClearProviders();

                        Log.Logger = loggerConfiguration.CreateLogger();

                        c.AddSerilog();
                    });
                    break;
                case LogTool.Default:
                default:
                    throw new InvalidOperationException($"Logging provider '{logConfiguration.Tool.Value}' unavailable.");
            }           

            return services;
        }
    }
}