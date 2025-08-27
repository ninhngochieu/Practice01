using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Elastic.Clients.Elasticsearch;
using System.Text.Json;
using Elastic.Clients.Elasticsearch.Analysis;
using Elastic.Clients.Elasticsearch.Mapping;
using Microsoft.Extensions.Configuration;
using Practice01.Infrastructure.Kafkaflow;

namespace Practice01.Infrastructure.Workers;

public class KafkaSerilogToElasticSearchWorker : BackgroundService
{
    private readonly ILogger<KafkaSerilogToElasticSearchWorker> _logger;
    private readonly ConsumerConfig _consumerConfig;
    private readonly string _topic;
    private readonly ElasticsearchClient _elasticClient;

    public KafkaSerilogToElasticSearchWorker(ILogger<KafkaSerilogToElasticSearchWorker> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _topic = "logs.app";

        // Cấu hình consumer để kết nối với Kafka
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = "logs.app-consumer-group",
            EnableAutoCommit = true,
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        // Cấu hình kết nối với Elasticsearch
        var settings =
            new ElasticsearchClientSettings(new Uri(configuration["Elasticsearch:Url"] ??
                                                    throw new InvalidOperationException()));
        _elasticClient = new ElasticsearchClient(settings);
    }

    //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //{
    //    _logger.LogInformation("Kafka to Elasticsearch worker running...");

    //    using var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
    //    consumer.Subscribe(_topic);

    //    try
    //    {
    //        while (!stoppingToken.IsCancellationRequested)
    //        {
    //            var consumeResult = consumer.Consume(stoppingToken);
    //            var logMessage = consumeResult.Message.Value;

    //            if (string.IsNullOrEmpty(logMessage))
    //            {
    //                continue;
    //            }

    //            try
    //            {
    //                var logDocument = JsonDocument.Parse(logMessage);
    //                var indexName = $"logs-{DateTime.UtcNow:yyyy.MM.dd}";

    //                // Kiểm tra và tạo index nếu cần
    //                await EnsureIndexExistsAsync(indexName, stoppingToken);

    //                // Gửi log đến Elasticsearch
    //                var response = await _elasticClient.IndexAsync(logDocument.RootElement, i => i.Index(indexName),
    //                    stoppingToken);

    //                if (!response.IsSuccess())
    //                {
    //                    _logger.LogError("Failed to index document to Elasticsearch. Error: {error}",
    //                        response.DebugInformation);
    //                }
    //                else
    //                {
    //                    _logger.LogInformation(
    //                        "Successfully indexed a log document to Elasticsearch index '{indexName}'.", indexName);
    //                }
    //            }
    //            catch (JsonException ex)
    //            {
    //                _logger.LogError(ex, "Failed to parse log message as JSON: {logMessage}", logMessage);
    //            }
    //            catch (Exception ex)
    //            {
    //                _logger.LogError(ex, "Error processing log message: {logMessage}", logMessage);
    //            }
    //        }
    //    }
    //    catch (OperationCanceledException)
    //    {
    //        _logger.LogInformation("Kafka consumer worker is stopping.");
    //    }
    //    finally
    //    {
    //        consumer.Close();
    //    }
    //}

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka to Elasticsearch worker running...");

        using var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
        consumer.Subscribe(_topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeBatch = consumer.ConsumeBatch(TimeSpan.FromSeconds(1), 100);
                if (consumeBatch.Count == 0)
                {
                    continue;
                }

                var logMessages = consumeBatch.Select(cr => cr.Message.Value)
                    .Where(msg => !string.IsNullOrEmpty(msg))
                    .ToList();
                if (logMessages.Count == 0)
                {
                    continue;
                }

                try
                {
                    var logDocuments = logMessages.Select(m => JsonDocument.Parse(m));
                    var indexName = $"logs-{DateTime.UtcNow:yyyy.MM.dd}";

                    // Kiểm tra và tạo index nếu cần
                    await EnsureIndexExistsAsync(indexName, stoppingToken);
                    var bulkResponse = await _elasticClient.BulkAsync(b => b.Index(indexName).IndexMany(logDocuments), stoppingToken);
                    if (bulkResponse.Errors)
                    {
                        foreach (var item in bulkResponse.ItemsWithErrors)
                        {
                            _logger.LogError($"Failed to index document {item.Id}: {item.Error}");
                        }
                    }

                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Failed to parse log message as JSON: {logMessages}", logMessages);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing log message: {logMessages}", logMessages);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka consumer worker is stopping.");
        }
        finally
        {
            consumer.Close();
        }
    }

    private readonly Dictionary<string, bool> _indexCache = new();

    /// <summary>
    /// Kiểm tra và tạo index nếu chưa tồn tại
    /// </summary>
    private async Task EnsureIndexExistsAsync(string indexName, CancellationToken cancellationToken)
    {
        // Kiểm tra cache để tránh gọi API không cần thiết
        if (_indexCache.ContainsKey(indexName))
        {
            return;
        }

        try
        {
            // Kiểm tra index có tồn tại không
            var existsResponse = await _elasticClient.Indices.ExistsAsync(indexName, cancellationToken);

            if (!existsResponse.Exists)
            {
                _logger.LogInformation("Index '{indexName}' does not exist. Creating...", indexName);

                // Tạo index với mapping và settings tùy chỉnh
                await CreateIndexWithMappingAsync(indexName, cancellationToken);

                _logger.LogInformation("Successfully created index '{indexName}'.", indexName);
            }

            // Lưu vào cache
            _indexCache[indexName] = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ensuring index '{indexName}' exists", indexName);
            throw;
        }
    }

    /// <summary>
    /// Tạo index với mapping và settings tùy chỉnh cho logs
    /// </summary>
    private async Task CreateIndexWithMappingAsync(string indexName, CancellationToken cancellationToken)
    {
        var createIndexResponse = await _elasticClient.Indices.CreateAsync(indexName, c => c
            .Settings(s => s
                .NumberOfShards(1)
                .NumberOfReplicas(1)
                .Analysis(a => a
                    .Analyzers(analyzers => analyzers
                        .Standard("log_analyzer", std => std.Stopwords(StopWordLanguage.English)
                        )
                    )
                )
            )
            .Mappings(m => m
                .Properties(p => p
                    .Date("Timestamp", dt => dt
                        .Format("strict_date_optional_time||epoch_millis")
                    )
                    // Trường Level dưới dạng keyword
                    .Keyword("Level")
                    // Trường Message với analyzer và sub-field
                    .Text("Message", t => t
                        .Analyzer("log_analyzer")
                        .Fields(f => f
                            .Keyword("messageTemplate")
                        )
                    )
                    .Keyword("SourceContext")
                    .Object("Properties", o => o
                        .Dynamic(DynamicMapping.True)
                    )
                    .Object("Exception", o => o
                        .Properties(ep => ep
                            .Text("Exception.Message")
                            .Keyword("Exception.Type")
                            .Text("Exception.StackTrace")
                        )
                    )
                )
            ), cancellationToken);

        if (!createIndexResponse.IsSuccess())
        {
            throw new InvalidOperationException(
                $"Failed to create index '{indexName}': {createIndexResponse.DebugInformation}"
            );
        }
    }
}