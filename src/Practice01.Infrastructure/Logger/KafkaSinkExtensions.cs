using Confluent.Kafka;
using Serilog;
using Serilog.Configuration;
using Serilog.Formatting;
using Serilog.Formatting.Json;

namespace Practice01.Infrastructure.Logger;

public static class KafkaSinkExtensions
{
    public static LoggerConfiguration Kafka(
        this LoggerSinkConfiguration loggerConfiguration,
        string bootstrapServers,
        string topic,
        ITextFormatter? formatter = null,
        Dictionary<string, string>? producerConfig = null)
    {
        var config = new ProducerConfig(producerConfig ?? new Dictionary<string, string>())
        {
            BootstrapServers = bootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true,
            LingerMs = 10,
            CompressionType = CompressionType.Lz4,
            MaxInFlight = 1
        };

        var producer = new ProducerBuilder<string, string>(config).Build();
        var sink = new KafkaSink(producer, topic, formatter ?? new JsonFormatter(renderMessage: true));
        return loggerConfiguration.Sink(sink);
    }
}