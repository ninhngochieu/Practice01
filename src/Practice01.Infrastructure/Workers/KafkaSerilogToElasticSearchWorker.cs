using Microsoft.Extensions.Hosting;

namespace Practice01.Infrastructure.Workers;

public class KafkaSerilogToElasticSearchWorker : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}