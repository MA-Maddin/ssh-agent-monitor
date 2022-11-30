using System.Text.Json.Serialization;

namespace SSHAgentMonitor
{
    public class MonitoringOptions
    {
        [JsonIgnore]
        public static string SectionKey => "Monitoring";

        public float Limit { get; set; } = 10.0f;       // CPU usage trigger limit in percent
        public int ExceedLimitCount { get; set; } = 3;  // How many consecutive times must the process exceed the limit before getting killed?
        public int CheckInterval { get; set; } = 10000; // Check interval in milliseconds
    }
}
