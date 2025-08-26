using KafkaFlow;
using KafkaFlow.Producers;
using Practice01.Application.Common.Producers;

namespace Practice01.Infrastructure.Kafkaflow.Producers;

public class TestMessageProducer : ITestMessageProducer
{
    private readonly IMessageProducer _producer;
    private const string ProducerName = "PrintConsole";
    private const string TopicName = "sample-topic";
    
    public TestMessageProducer(IProducerAccessor producerAccessor)
    {
        _producer = producerAccessor.GetProducer(ProducerName);   
    }
    public Task ProduceAsync<T>(string key, T message)
    {
        _producer.ProduceAsync(TopicName, key, message);
        return Task.CompletedTask;
    }
}