using Confluent.Kafka;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace Practice01.Infrastructure.Logger;

public class KafkaSink : ILogEventSink
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;
    private readonly ITextFormatter _formatter;

    public KafkaSink(IProducer<string, string> producer, string topic, ITextFormatter formatter)
    {
        _producer = producer;
        _topic = topic;
        _formatter = formatter;
    }

    public void Emit(LogEvent logEvent)
    {
        using var writer = new StringWriter();
        _formatter.Format(logEvent, writer);
        var json = writer.ToString();

        var key = logEvent.Properties.TryGetValue("traceId", out var property)
            ? property.ToString().Trim('"')
            : null;

        _producer.Produce(_topic, new Message<string, string>
        {
            Key = key,
            Value = json
        });
    }
}