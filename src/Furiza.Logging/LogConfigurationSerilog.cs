using System.Collections.Generic;

namespace Furiza.Logging
{
    public class LogConfigurationSerilog
    {
        public IEnumerable<string> Using { get; set; }
        public LogConfigurationSerilogMinimumLevel MinimumLevel { get; set; }
        public IEnumerable<LogConfigurationSerilogWriteTo> WriteTo { get; set; }
        public IEnumerable<string> Enrich { get; set; }
    }
}