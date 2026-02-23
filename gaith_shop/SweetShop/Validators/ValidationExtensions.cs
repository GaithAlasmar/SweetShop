using FluentValidation;
using FluentValidation.Results;

namespace SweetShop.Validators;

/// <summary>
/// Extension methods to map FluentValidation failures into
/// ASP.NET Core ModelState for automatic Razor view error display.
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Adds FluentValidation errors to ModelState so they appear
    /// in Razor views via @Html.ValidationMessageFor() / asp-validation-for.
    /// </summary>
    public static void AddToModelState(
        this ValidationResult result,
        Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState)
    {
        foreach (var error in result.Errors)
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
    }

    /// <summary>
    /// Converts a FluentValidation ValidationException into a
    /// structured dictionary keyed by property name.
    /// Useful for JSON API responses.
    ///
    /// Example output:
    /// {
    ///   "Email":    ["هذا البريد الإلكتروني مسجل مسبقاً"],
    ///   "Password": ["يجب أن تحتوي كلمة المرور على رمز خاص"]
    /// }
    /// </summary>
    public static Dictionary<string, string[]> ToErrorDictionary(
        this ValidationException ex)
        => ex.Errors
             .GroupBy(f => f.PropertyName)
             .ToDictionary(
                 g => g.Key,
                 g => g.Select(f => f.ErrorMessage).ToArray());
}
