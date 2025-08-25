using System.Net;
using MediatR;
using Practice01.Application.Common.File;
using Practice01.Application.Common.Validation;

namespace Practice01.Application.TestFileService.Queries.Excel;

public class TestGetJsonFileQuery : IRequest<Stream>
{
    public string FileName { get; set; }
}

public class TestGetJsonFileQueryHandler : IRequestHandler<TestGetJsonFileQuery, Stream>
{
    private readonly IFileService _fileService;
    private readonly ErrorCollector _errorCollector;

    public TestGetJsonFileQueryHandler(IFileService fileService, ErrorCollector errorCollector)
    {
        _fileService = fileService;
        _errorCollector = errorCollector;
    }
    public async Task<Stream> Handle(TestGetJsonFileQuery request, CancellationToken cancellationToken)
    {
        var fileName = Path.Combine("/","tmp",request.FileName);
        var stream = await _fileService.ReadJsonAsync(fileName);
        _errorCollector.Success(HttpStatusCode.OK, "TEST_GET_JSON_FILE_SUCCESS", "Test Get Json File Success");
        return stream;
    }
}