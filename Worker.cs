using AddressIPControlBackgroundService.Helpers;

namespace AddressIPControlBackgroundService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Start us³ugi!");
        NetManager netManager = new NetManager(_logger);
        await netManager.Load();

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Kolejne wywo³anie");
            await netManager.CheckMyPublicIPAddressAsync();
            await Task.Delay(60000, stoppingToken);
        }
    }
}
