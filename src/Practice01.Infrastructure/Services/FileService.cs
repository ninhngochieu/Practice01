using System.Text.Json;
using OfficeOpenXml;
using Practice01.Application.Common.File;

namespace Practice01.Infrastructure.Services;

public class FileService : IFileService
{
    public Task WriteExcelAsync<T>(string fileName, IEnumerable<T> data, string sheetName = "Sheet1")
    {
        return Task.Run(() =>
        {
            var fileInfo = new FileInfo(fileName);
            fileInfo.Directory?.Create();

            using var package = new ExcelPackage(fileInfo);
            // Xóa sheet cũ nếu đã tồn tại
            var existing = package.Workbook.Worksheets[sheetName];
            if (existing != null)
            {
                package.Workbook.Worksheets.Delete(existing);
            }

            // Tạo sheet mới
            var ws = package.Workbook.Worksheets.Add(sheetName);

            // Load dữ liệu với header (true)
            ws.Cells["A1"].LoadFromCollection(data, true);

            // Tự động điều chỉnh cột
            ws.Cells.AutoFitColumns();

            package.Save();
        });
    }

    public Task WriteJsonAsync<T>(string fileName, IEnumerable<T> data)
    {
        var json = JsonSerializer.Serialize(data);
        return File.WriteAllTextAsync(fileName, json);
    }
}