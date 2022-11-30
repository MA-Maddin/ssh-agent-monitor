using SSHAgentMonitor;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.Configure<MonitoringOptions>(configuration.GetSection(MonitoringOptions.SectionKey));

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
