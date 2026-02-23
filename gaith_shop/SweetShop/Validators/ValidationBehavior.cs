using FluentValidation;
using MediatR;

namespace SweetShop.Validators;

/// <summary>
/// MediatR Pipeline Behavior that runs FluentValidation automatically
/// before any IRequest handler executes.
///
/// Flow:
///   Controller → mediator.Send(request)
///       → ValidationBehavior (validates here)
///           → If errors: throw ValidationException (structured errors)
///           → If valid:  call next() → actual handler
///
/// Register once in Program.cs — applies to ALL MediatR commands and queries
/// that have a matching IValidator<TRequest> registered.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(); // No validator registered — pass through

        var context = new ValidationContext<TRequest>(request);

        // Run all matching validators in parallel
        var results = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Collect all failures across all validators
        var failures = results
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count != 0)
        {
            // Group by property name for structured output
            // { "Email": ["...", "..."], "Password": ["..."] }
            var grouped = failures
                .GroupBy(f => f.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(f => f.ErrorMessage).ToArray());

            throw new FluentValidation.ValidationException(failures);
        }

        return await next();
    }
}
