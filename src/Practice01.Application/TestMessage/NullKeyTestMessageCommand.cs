using System.Net;
using MediatR;
using Practice01.Application.Common.Producers;
using Practice01.Application.Common.Validation;

namespace Practice01.Application.TestMessage;

public class NullKeyTestMessageCommand : IRequest
{
    
}

public class TestMessageCommandHandler : IRequestHandler<NullKeyTestMessageCommand>
{
    private readonly ErrorCollector _errorCollector;
    private readonly ITestMessageProducer _testMessageProducer;

    public TestMessageCommandHandler(ErrorCollector errorCollector, ITestMessageProducer testMessageProducer)
    {
        _errorCollector = errorCollector;
        _testMessageProducer = testMessageProducer;
    }
    public Task Handle(NullKeyTestMessageCommand request, CancellationToken cancellationToken)
    {
        _testMessageProducer.ProduceAsync(null, new Common.Producers.TestMessage()
        {
            Text = $"Message {Guid.NewGuid()}"
        });
        _errorCollector.Success(HttpStatusCode.OK, "TEST_MESSAGE_SUCCESS", "Test Message Success");
        return Task.CompletedTask;
    }
}