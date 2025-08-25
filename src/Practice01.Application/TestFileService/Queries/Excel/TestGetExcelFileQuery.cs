using System.Net;
using MediatR;
using Practice01.Application.Common.File;
using Practice01.Application.Common.Validation;

namespace Practice01.Application.TestFileService.Queries.Excel;

public class TestGetExcelFileQuery : IRequest<Stream>
{
    public string FileName { get; set; }
}

public class TestGetExcelFileQueryHandler : IRequestHandler<TestGetExcelFileQuery, Stream>
{
    private readonly IFileService _fileService;
    private readonly ErrorCollector _errorCollector;

    public TestGetExcelFileQueryHandler(IFileService fileService, ErrorCollector errorCollector)
    {
        _fileService = fileService;
        _errorCollector = errorCollector;
    }
    public async Task<Stream> Handle(TestGetExcelFileQuery request, CancellationToken cancellationToken)
    {
        var fileName = Path.Combine("/","tmp",request.FileName);
        _errorCollector.Success(HttpStatusCode.OK, "TEST_GET_JSON_FILE_SUCCESS", "Test Get Json File Success");
        return  await _fileService.ReadExcelAsync(fileName);;
    }
}