using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Practice01.Application.Common.Log;

namespace Practice01.Infrastructure.Workers;

public class SerilogToKafkaLogWorker : BackgroundService
{
    private readonly ILogger<SerilogToKafkaLogWorker> _logger;

    public SerilogToKafkaLogWorker(ILogger<SerilogToKafkaLogWorker> logger)
    {
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}