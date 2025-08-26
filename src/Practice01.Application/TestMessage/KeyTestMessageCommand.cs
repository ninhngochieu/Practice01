using System.Net;
using MediatR;
using Practice01.Application.Common.Producers;
using Practice01.Application.Common.Validation;

namespace Practice01.Application.TestMessage;

public class KeyTestMessageCommand : IRequest
{
    public string Key { get; set; }
}

public class KeyTestMessageCommandHandler : IRequestHandler<KeyTestMessageCommand>
{
    private readonly ErrorCollector _errorCollector;
    private readonly ITestMessageProducer _testMessageProducer;

    public KeyTestMessageCommandHandler(ErrorCollector errorCollector, ITestMessageProducer testMessageProducer)
    {
        _errorCollector = errorCollector;
        _testMessageProducer = testMessageProducer;
    }
    public Task Handle(KeyTestMessageCommand request, CancellationToken cancellationToken)
    {
        _testMessageProducer.ProduceAsync(request.Key, new Common.Producers.TestMessage()
        {
            Text = $"Message {Guid.NewGuid()}"
        });
        _errorCollector.Success(HttpStatusCode.OK, "TEST_MESSAGE_SUCCESS", "Test Message Success");
        return Task.CompletedTask;
    }
}