using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Practice01.Application.Common.Validation;

namespace Practice01.Presentation.Common.ObjectResult;

public class CustomObjectResult
{
    private readonly ErrorCollector _errorCollector;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomObjectResult(ErrorCollector errorCollector, IHttpContextAccessor httpContextAccessor)
    {
        _errorCollector = errorCollector;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public IResult Return<T>(T data)
    {
        var apiResponse = new ApiResponse<T>
        {
            Ok = _errorCollector.Ok,
            StatusCode = _errorCollector.StatusCode,
            Message = _errorCollector.Message,
            Code = _errorCollector.Code,
            Data = data,
            Metadata = new Metadata
            {
                RequestId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? string.Empty,
                TraceId = Activity.Current?.Id ?? string.Empty
            },
            Details = _errorCollector.Details
        };

        return Results.Json(apiResponse, statusCode: _errorCollector.StatusCode);
    }

    public IResult Return()
    {
        var apiResponse = new ApiResponse
        {
            Ok = _errorCollector.Ok,
            StatusCode = _errorCollector.StatusCode,
            Message = _errorCollector.Message,
            Code = _errorCollector.Code,
            Metadata = new Metadata
            {
                RequestId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? string.Empty,
                TraceId = Activity.Current?.Id ?? string.Empty
            },
            Details = _errorCollector.Details
        };

        return Results.Json(apiResponse, statusCode: _errorCollector.StatusCode);
    }
}