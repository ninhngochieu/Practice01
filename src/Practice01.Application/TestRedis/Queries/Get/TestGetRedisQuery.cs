using System.Net;
using MediatR;
using Practice01.Application.Common.Cache;
using Practice01.Application.Common.Validation;

namespace Practice01.Application.TestApiKey.Queries.TestRedis.Get;

public class TestGetRedisQuery : IRequest<string>
{
    public TestGetRedisQuery(string key)
    {
        Key = key;
    }

    public string Key { get; set; }
}

public class TestGetRedisQueryHandler : IRequestHandler<TestGetRedisQuery, string?>
{
    private readonly IRedisCacheService _redisCacheService;
    private readonly ErrorCollector _errorCollector;

    public TestGetRedisQueryHandler(IRedisCacheService redisCacheService, ErrorCollector errorCollector)
    {
        _redisCacheService = redisCacheService;
        _errorCollector = errorCollector;
    }

    public async Task<string?> Handle(TestGetRedisQuery request, CancellationToken cancellationToken)
    {
        _errorCollector.Success(HttpStatusCode.OK, "TEST_GET_REDIS_SUCCESS", "Test Get Redis Success");
        return await _redisCacheService.GetAsync<string>(request.Key);
    }
}