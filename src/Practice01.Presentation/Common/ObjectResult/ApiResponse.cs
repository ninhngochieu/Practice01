using System.Net;
using Practice01.Application.Common.Validation;

namespace Practice01.Presentation.Common.ObjectResult;

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }
}

public class ApiResponse
{
    public bool Ok { get; set; } = true;
    public int StatusCode { get; set; } = (int)HttpStatusCode.OK;
    public string Message { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public List<CollectedError> Details { get; set; } = [];
    public Metadata Metadata { get; set; } = new();
}