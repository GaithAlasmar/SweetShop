namespace SweetShop.ViewModels;

/// <summary>
/// Enhanced registration model with full name and phone number.
/// Replaces DataAnnotations — all validation is handled by FluentValidation.
/// </summary>
public class RegisterViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
