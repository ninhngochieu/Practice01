using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice01.Infrastructure.Kafkaflow;
internal static class ConsumerExtensions
{
    public static IReadOnlyCollection<ConsumeResult<TKey, TValue>> ConsumeBatch<TKey, TValue>(this IConsumer<TKey, TValue> consumer, TimeSpan consumeTimeout, int maxBatchSize)
    {
        var message = consumer.Consume(consumeTimeout);

        if (message?.Message is null)
        {
            return Array.Empty<ConsumeResult<TKey, TValue>>();
        }

        var messageBatches = new List<ConsumeResult<TKey, TValue>> { message };

        while (messageBatches.Count < maxBatchSize)
        {
            message = consumer.Consume(TimeSpan.Zero);
            if (message?.Message is null)
            {
                break;
            }

            messageBatches.Add(message);
        }

        return messageBatches;
    }
}
