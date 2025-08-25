using System.Net;
using MediatR;
using Practice01.Application.Common.File;
using Practice01.Application.Common.Validation;

namespace Practice01.Application.TestFileService.Commands.Json;

public class TestWriteJsonFileCommand : IRequest<string>
{
    
}

public class TestWriteJsonFileCommandHandler : IRequestHandler<TestWriteJsonFileCommand, string>
{
    private readonly ErrorCollector _errorCollector;
    private readonly IFileService _fileService;

    public TestWriteJsonFileCommandHandler(ErrorCollector errorCollector, IFileService fileService)
    {
        _errorCollector = errorCollector;
        _fileService = fileService;
    }
    public async Task<string> Handle(TestWriteJsonFileCommand request, CancellationToken cancellationToken)
    {
        var tempFile = Path.GetTempFileName();
        var fileName = Path.ChangeExtension(tempFile, ".json");
        var data = new[]
        {
            new { Name = "John", Age = 30 },
            new { Name = "Jane", Age = 25 },
            new { Name = "Bob", Age = 35 },
        };
        
        await _fileService.WriteJsonAsync(fileName, data);
        _errorCollector.Success(HttpStatusCode.OK, "TEST_WRITE_JSON_FILE_SUCCESS", "Test Write Json File Success");
        return fileName;
    }
}