using FluentValidation;
using MediatR;
using Practice01.Application.Common.Validation;

namespace Practice01.Application.Common.Behaviours;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ErrorCollector _errorCollector;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ErrorCollector errorCollector)
    {
        _validators = validators;
        _errorCollector = errorCollector;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next(cancellationToken);
        }

        var collectedErrors = failures
            .Select(f => new CollectedError
            {
                Code = f.ErrorCode,
                Field = f.PropertyName,
                Message = f.ErrorMessage
            })
            .ToList();
        
        _errorCollector.AddValidationError(collectedErrors);
        throw new ValidationException(failures);
    }
}