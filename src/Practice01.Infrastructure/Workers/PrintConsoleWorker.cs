using KafkaFlow;
using KafkaFlow.Producers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Practice01.Application.Common.Producers;

namespace Practice01.Infrastructure.Workers;

public class PrintConsoleWorker : BackgroundService
{
    private readonly ILogger<PrintConsoleWorker> _logger;
    private readonly IMessageProducer _producer;

    private const string ProducerName = "PrintConsole";
    private const string TopicName = "sample-topic";

    public PrintConsoleWorker(ILogger<PrintConsoleWorker> logger, IProducerAccessor producerAccessor)
    {
        _logger = logger;
         _producer = producerAccessor.GetProducer(ProducerName);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {0}", DateTimeOffset.Now);
            
            await _producer.ProduceAsync(
                TopicName,
                Guid.NewGuid().ToString(),
                new TestMessage { Text = $"Message: {Guid.NewGuid()}" });
            await Task.Delay(1000, stoppingToken);
        }
    }
}