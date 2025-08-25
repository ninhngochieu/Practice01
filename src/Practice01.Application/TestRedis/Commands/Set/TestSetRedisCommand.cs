using System.Net;
using MediatR;
using Practice01.Application.Common.Cache;
using Practice01.Application.Common.Validation;

namespace Practice01.Application.TestApiKey.Queries.TestRedis.Set;

public class TestSetRedisCommand : IRequest
{
    public string Value { get; set; }

    public string Key { get; set; }
}

public class TestSetRedisCommandHandler : IRequestHandler<TestSetRedisCommand>
{
    private readonly IRedisCacheService _redisCacheService;
    private readonly ErrorCollector _errorCollector;

    public TestSetRedisCommandHandler(IRedisCacheService redisCacheService, ErrorCollector errorCollector)
    {
        _redisCacheService = redisCacheService;
        _errorCollector = errorCollector;
    }

    public async Task Handle(TestSetRedisCommand request, CancellationToken cancellationToken)
    {
        _errorCollector.Success(HttpStatusCode.NoContent, "TEST_SET_REDIS_SUCCESS", "Test Set Redis Success");
        await _redisCacheService.SetAsync(request.Key, request.Value);
    }
}