using System.Net;
using MediatR;
using Practice01.Application.Common.Cache;
using Practice01.Application.Common.Validation;

namespace Practice01.Application.TestRedis.Commands.Delete;

public class TestDeleteRedisCommand : IRequest
{
    public TestDeleteRedisCommand(string key)
    {
        Key = key;
    }

    public string Key { get; set; }
}

public class TestDeleteRedisCommandHandler : IRequestHandler<TestDeleteRedisCommand>
{
    private readonly IRedisCacheService _redisCacheService;
    private readonly ErrorCollector _errorCollector;

    public TestDeleteRedisCommandHandler(IRedisCacheService redisCacheService, ErrorCollector errorCollector)
    {
        _redisCacheService = redisCacheService;
        _errorCollector = errorCollector;
    }
    public async Task Handle(TestDeleteRedisCommand request, CancellationToken cancellationToken)
    {
        var ok = await _redisCacheService.KeyDeleteAsync(request.Key);
        if (!ok)
        {
            _errorCollector.Error(HttpStatusCode.BadRequest, "FAILED_TO_DELETE_REDIS_KEY", "Failed to delete redis key");
            return;
        }
        _errorCollector.Success(HttpStatusCode.NoContent, "TEST_DELETE_REDIS_SUCCESS", "Test Delete Redis Success");
        return;
    }
}