using FiloShop.SharedKernel.Errors;
using FluentValidation;
using MediatR;
using ValidationException = FiloShop.SharedKernel.Exceptions.ValidationException;

namespace FiloShop.SharedKernel.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators = null!)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        var validationErrors = _validators.Select(v => v.Validate(context)
            ).Where(validationResult => validationResult.Errors.Count != 0)
            .SelectMany(validationResult => validationResult.Errors)
            .Select(validationFailure => new ValidationError(
                validationFailure.PropertyName,
                validationFailure.ErrorMessage)).ToList();

        if (validationErrors.Count != 0) throw new ValidationException(validationErrors);
        return await next(cancellationToken);
    }
}