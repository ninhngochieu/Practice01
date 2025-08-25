using System.Net;
using MediatR;
using Practice01.Application.Common.File;
using Practice01.Application.Common.Validation;

namespace Practice01.Application.TestFileService.Queries;

public class TestWriteExcelFileCommand : IRequest<string>
{
    
}

public class TestWriteExcelFileCommandHandler : IRequestHandler<TestWriteExcelFileCommand, string>
{
    private readonly IFileService _fileService;
    private readonly ErrorCollector _errorCollector;

    public TestWriteExcelFileCommandHandler(IFileService fileService, ErrorCollector errorCollector)
    {
        _fileService = fileService;
        _errorCollector = errorCollector;
    }
    public async Task<string> Handle(TestWriteExcelFileCommand request, CancellationToken cancellationToken)
    {
        var tempFile = Path.GetTempFileName();
        var fileName = Path.ChangeExtension(tempFile, ".xlsx");
        var data = new[]
        {
            new { Name = "John", Age = 30 },
            new { Name = "Jane", Age = 25 },
            new { Name = "Bob", Age = 35 },
        };
        
        await _fileService.WriteExcelAsync(fileName, data);
        _errorCollector.Success(HttpStatusCode.OK, "TEST_WRITE_EXCEL_FILE_SUCCESS", "Test Write Excel File Success");
        return fileName;
    }
}