using System.Net;

namespace Practice01.Application.Common.Validation;

public class ErrorCollector
{
    public bool Ok { get; set; } = true;
    public int StatusCode { get; set; } = 200;
    public string Message { get; set; } = string.Empty;
    public string Code { get; set; } = "SUCCESS";

    public List<CollectedError> Details { get; set; } = [];
        
    public void Success(string code, string message)
    {
        Ok = true;
        Message = message;
        StatusCode = (int)HttpStatusCode.OK;
        Code = code;
    }

    public void Success(string message)
    {
        Ok = true;
        Message = message;
        StatusCode = (int)HttpStatusCode.OK;
        Code = "SUCCESS";
    }

    public void Success(HttpStatusCode statusCode, string message)
    {
        Ok = true;
        Message = message;
        StatusCode = statusCode switch
        {
            HttpStatusCode.OK
                or HttpStatusCode.Created
                or HttpStatusCode.Accepted
                or HttpStatusCode.NoContent
                or HttpStatusCode.Redirect => (int)statusCode,
            _ => throw new ArgumentOutOfRangeException(nameof(statusCode), statusCode, null)
        };
        Code = "SUCCESS";
    }

    public void Success(HttpStatusCode statusCode, string code, string message)
    {
        Ok = true;
        Message = message;
        StatusCode = statusCode switch
        {
            HttpStatusCode.OK
                or HttpStatusCode.Created
                or HttpStatusCode.Accepted
                or HttpStatusCode.NoContent
                or HttpStatusCode.Redirect => (int)statusCode,
            _ => throw new ArgumentOutOfRangeException(nameof(statusCode), statusCode, null)
        };
        Code = string.IsNullOrEmpty(code) ? "SUCCESS" : code.ToUpper();
    }

    public void AddValidationError(List<CollectedError> collectedErrors)
    {
        Ok = false;
        StatusCode = (int)HttpStatusCode.BadRequest;
        Code = "VALIDATION_ERROR";
        Message = "Validation Error";
        Details = collectedErrors;
    }

    public void Error(string message)
    {
        Ok = false;
        StatusCode = (int)HttpStatusCode.BadRequest;
        Code = "ERROR";
        Message = message;
        Details = [];
    }

    public void Error(string code, string message)
    {
        Ok = false;
        StatusCode = (int)HttpStatusCode.BadRequest;
        Code = code;
        Message = message;
        Details = [];
    }

    public void Error(HttpStatusCode statusCode, string code, string message)
    {
        Ok = false;
        Code = code;
        Message = message;
        Details = [];
        StatusCode = statusCode switch
        {
            HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden
                or HttpStatusCode.NotFound or HttpStatusCode.UnsupportedMediaType
                or HttpStatusCode.Conflict => (int)statusCode,
            _ => throw new ArgumentOutOfRangeException(nameof(statusCode), statusCode, null)
        };
    }
}