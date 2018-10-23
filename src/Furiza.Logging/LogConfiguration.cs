using System.ComponentModel.DataAnnotations;

namespace Furiza.Logging
{
    public class LogConfiguration
    {
        [Required]
        public LogTool? Tool { get; set; }

        public LogWriteTo WriteToType { get; set; } = LogWriteTo.Console;
    }
}