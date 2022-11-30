using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace SSHAgentMonitor
{
    public class Worker : BackgroundService, IDisposable
    {
        private const string SSHAgentExeName = "ssh-agent.exe";
        private const string SSHAgentProcessName = "ssh-agent";
        private const string CategoryName = "Process";
        private const string CounterName = "% Processor Time";

        private readonly PerformanceCounterCategory _pcc = new PerformanceCounterCategory(CategoryName);
        private readonly ILogger<Worker> _logger;
        private readonly IDisposable _monitorOptionsChangeListener;
        private MonitoringOptions _monitorOptions;
        private int _exceedLimitCounter;

        public Worker(
            ILogger<Worker> logger,
            IOptionsMonitor<MonitoringOptions> monitorOptions)
        {
            _logger = logger;
            _monitorOptions = monitorOptions.CurrentValue;
            _monitorOptionsChangeListener = monitorOptions.OnChange(MonitorOptionsChanged);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ResetCounter();

            var sshAgentCpu = new PerformanceCounter(CategoryName, CounterName, SSHAgentProcessName, true);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_pcc.InstanceExists(SSHAgentProcessName))
                    {
                        float sshUsage = sshAgentCpu.NextValue() / Environment.ProcessorCount;
                        _logger.LogInformation($"SSH Agent CPU: {sshUsage} %");

                        if (sshUsage > _monitorOptions.Limit)
                        {
                            _exceedLimitCounter++;
                            if (_exceedLimitCounter > _monitorOptions.ExceedLimitCount)
                            {
                                KillSshAgent();
                            }
                        }
                        else
                        {
                            ResetCounter();
                        }
                    }
                    else
                    {
                        _logger.LogInformation("No SSH Agent running");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception occured: {ex.Message}");
                }

                await Task.Delay(_monitorOptions.CheckInterval, stoppingToken);
            }
        }

        private void KillSshAgent()
        {
            try
            {
                Process.Start("cmd.exe", $"/c taskkill /F /IM {SSHAgentExeName}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not kill SSH Agent. Exception: {ex.Message}");
            }

            ResetCounter();
        }

        private void ResetCounter()
        {
            _exceedLimitCounter = 0;
        }

        private void MonitorOptionsChanged(MonitoringOptions monitorOptions, string arg2)
        {
            _monitorOptions = monitorOptions;
            _logger.LogInformation($"{nameof(MonitoringOptions)} have changed. Loaded new configuration.");
        }

        public override void Dispose()
        {
            _monitorOptionsChangeListener?.Dispose();
        }
    }
}