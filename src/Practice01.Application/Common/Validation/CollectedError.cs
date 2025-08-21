namespace Practice01.Application.Common.Validation;

public class CollectedError
{
    public string Code { get; set; } = string.Empty;
    public string  Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}