using System.Diagnostics;

namespace SSHAgentMonitor
{
    public class Worker : BackgroundService
    {
        private const string SSHAgentExeName = "ssh-agent.exe";
        private const string SSHAgentProcessName = "ssh-agent";
        private const float Limit = 10; // CPU usage trigger limit in percent
        private const int MinCounter = 10; // How many consecutive times must the process exceed the limit before getting killed?
        private const int Delay = 1000; // Check interval in milliseconds

        private const string CategoryName = "Process";
        private const string CounterName = "% Processor Time";

        private readonly PerformanceCounterCategory _pcc = new PerformanceCounterCategory(CategoryName);
        private readonly ILogger<Worker> _logger;
        private int _exceedLimitCounter;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
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

                        if (sshUsage > Limit)
                        {
                            _exceedLimitCounter++;
                            if (_exceedLimitCounter > MinCounter)
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

                await Task.Delay(Delay, stoppingToken);
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
    }
}