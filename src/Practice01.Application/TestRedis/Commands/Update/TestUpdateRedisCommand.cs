using System.Net;
using MediatR;
using Practice01.Application.Common.Cache;
using Practice01.Application.Common.Validation;

namespace Practice01.Application.TestApiKey.Queries.TestRedis.Set;

public class TestUpdateRedisCommand : IRequest
{
    public string Value { get; set; }

    public string Key { get; set; }
}

public class TestUpdateRedisCommandHandler : IRequestHandler<TestUpdateRedisCommand>
{
    private readonly IRedisCacheService _redisCacheService;
    private readonly ErrorCollector _errorCollector;

    public TestUpdateRedisCommandHandler(IRedisCacheService redisCacheService, ErrorCollector errorCollector)
    {
        _redisCacheService = redisCacheService;
        _errorCollector = errorCollector;
    }

    public async Task Handle(TestUpdateRedisCommand request, CancellationToken cancellationToken)
    {
        var exist = await _redisCacheService.KeyExistsAsync(request.Key);
        if (!exist)
        {
            _errorCollector.Error(HttpStatusCode.NotFound, "REDIS_KEY_NOT_FOUND", "Redis key not found");
            return;
        }
        
        _errorCollector.Success(HttpStatusCode.NoContent, "TEST_UPDATE_REDIS_SUCCESS", "Test Update Redis Success");
        await _redisCacheService.SetAsync(request.Key, request.Value);
    }
}