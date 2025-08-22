using System.Net;
using MediatR;
using Practice01.Application.Common.Validation;

namespace Practice01.Application.TestApiKey.Queries.TestApiKey;

public class TestApiKeyCommand : IRequest<string>
{
    
}

public class TestApiKeyCommandHandler : IRequestHandler<TestApiKeyCommand, string>
{
    private readonly ErrorCollector _collector;

    public TestApiKeyCommandHandler(ErrorCollector collector)
    {
        _collector = collector;
    }
    public Task<string> Handle(TestApiKeyCommand request, CancellationToken cancellationToken)
    {
        _collector.Success(HttpStatusCode.OK, "TEST_API_KEY_SUCCESS", "Test ApiKey Success");
        return Task.FromResult("Success");
    }
}