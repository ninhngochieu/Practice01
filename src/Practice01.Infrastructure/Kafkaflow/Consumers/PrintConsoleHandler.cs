using KafkaFlow;
using Microsoft.Extensions.Logging;
using Practice01.Application.Common.Producers;

namespace Practice01.Infrastructure.Kafkaflow.Consumers;

public class PrintConsoleHandler : IMessageHandler<TestMessage>
{
    private readonly ILogger<PrintConsoleHandler> _logger;

    public PrintConsoleHandler(ILogger<PrintConsoleHandler> logger)
    {
        _logger = logger;
    }
    public Task Handle(IMessageContext context, TestMessage message)
    {
        _logger.LogInformation(
            "Partition: {0} | Offset: {1} | Message: {2}",
            context.ConsumerContext.Partition,
            context.ConsumerContext.Offset,
            message.Text);

        return Task.CompletedTask;
    }
}