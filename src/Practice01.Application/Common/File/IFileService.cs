namespace Practice01.Application.Common.File;

/// <summary>
/// Add common function in order to write excel or json file
/// </summary>
public interface IFileService
{
    Task WriteExcelAsync<T>(string fileName, IEnumerable<T> data, string sheetName = "Sheet1");
    Task WriteJsonAsync<T>(string fileName, IEnumerable<T> data);
    
    Task<Stream> ReadExcelAsync(string fileName);
    Task<Stream> ReadJsonAsync(string fileName);
}