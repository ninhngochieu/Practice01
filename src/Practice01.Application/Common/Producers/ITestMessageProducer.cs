namespace Practice01.Application.Common.Producers;

public interface ITestMessageProducer
{
    Task ProduceAsync<T>(string key, T message);
}