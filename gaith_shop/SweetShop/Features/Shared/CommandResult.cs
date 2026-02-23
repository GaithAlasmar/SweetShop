namespace SweetShop.Features.Shared;

/// <summary>
/// A lightweight result type returned by Command handlers.
/// Controllers check Success and act accordingly.
/// </summary>
public record CommandResult(bool Success, string? ErrorMessage = null)
{
    public static CommandResult Ok() => new(true);
    public static CommandResult Fail(string error) => new(false, error);
}
