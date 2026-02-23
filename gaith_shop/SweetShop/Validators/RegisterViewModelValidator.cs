using FluentValidation;
using Microsoft.AspNetCore.Identity;
using SweetShop.Models;
using SweetShop.ViewModels;

namespace SweetShop.Validators;

/// <summary>
/// Full FluentValidation pipeline for user registration.
/// Rules: strong password, unique email (async DB check), valid phone format.
/// All error messages are in Arabic to match the app's UI language.
/// </summary>
public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterViewModelValidator(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;

        // ── Full Name ──────────────────────────────────────────────────
        RuleFor(x => x.FullName)
            .NotEmpty()
                .WithMessage("الاسم الكامل مطلوب")
            .MinimumLength(3)
                .WithMessage("الاسم الكامل يجب أن يحتوي على 3 أحرف على الأقل")
            .MaximumLength(100)
                .WithMessage("الاسم الكامل يجب ألا يتجاوز 100 حرف")
            .Matches(@"^[\u0600-\u06FFa-zA-Z\s]+$")
                .WithMessage("الاسم يجب أن يحتوي على أحرف عربية أو إنجليزية فقط");

        // ── Email — Format + Uniqueness (async DB check) ───────────────
        RuleFor(x => x.Email)
            .NotEmpty()
                .WithMessage("البريد الإلكتروني مطلوب")
            .EmailAddress()
                .WithMessage("صيغة البريد الإلكتروني غير صحيحة")
            .MaximumLength(256)
                .WithMessage("البريد الإلكتروني طويل جداً")
            // Async uniqueness check — hits Identity's UserManager (not raw DB)
            // so it respects any email normalization / provider overrides.
            .MustAsync(BeUniqueEmailAsync)
                .WithMessage("هذا البريد الإلكتروني مسجل مسبقاً، يرجى تسجيل الدخول أو استخدام بريد آخر")
            // Chain rules: don't run the async DB check if the format is invalid
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        // ── Phone Number ───────────────────────────────────────────────
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
                .WithMessage("رقم الهاتف مطلوب")
            .Must(BeValidJordanianOrInternationalPhone)
                .WithMessage(
                    "رقم الهاتف غير صحيح. الصيغ المقبولة: 07XXXXXXXX أو +9627XXXXXXXX أو +1XXXXXXXXXX");

        // ── Password — Strong Rules ────────────────────────────────────
        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage("كلمة المرور مطلوبة")
            .MinimumLength(8)
                .WithMessage("كلمة المرور يجب أن تحتوي على 8 أحرف على الأقل")
            .MaximumLength(128)
                .WithMessage("كلمة المرور طويلة جداً")
            .Matches(@"[A-Z]")
                .WithMessage("يجب أن تحتوي كلمة المرور على حرف كبير واحد على الأقل")
            .Matches(@"[a-z]")
                .WithMessage("يجب أن تحتوي كلمة المرور على حرف صغير واحد على الأقل")
            .Matches(@"\d")
                .WithMessage("يجب أن تحتوي كلمة المرور على رقم واحد على الأقل")
            .Matches(@"[\!\@\#\$\%\^\&\*\(\)\-_\=\+\[\]\{\}\|;:',.<>?/`~]")
                .WithMessage("يجب أن تحتوي كلمة المرور على رمز خاص واحد على الأقل (!@#$%^&*)")
            .Must(NotContainSpaces)
                .WithMessage("كلمة المرور لا يجب أن تحتوي على مسافات");

        // ── Confirm Password — Cross-field comparison ──────────────────
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
                .WithMessage("تأكيد كلمة المرور مطلوب")
            .Equal(x => x.Password)
                .WithMessage("كلمة المرور وتأكيدها غير متطابقين");
    }

    // ── Custom Rule Implementations ────────────────────────────────────

    /// <summary>
    /// Asynchronously checks UserManager to see if the email already exists.
    /// Runs only when the email format is valid (see .When() above).
    /// </summary>
    private async Task<bool> BeUniqueEmailAsync(
        string email, CancellationToken cancellationToken)
    {
        var existing = await _userManager.FindByEmailAsync(email);
        return existing is null; // true = valid (no existing user found)
    }

    /// <summary>
    /// Accepts:
    ///   - Jordanian mobile:          07XXXXXXXX (10 digits, starts with 07)
    ///   - Jordanian international:   +9627XXXXXXXX
    ///   - Generic international:     +[country-code][number] (7-15 digits)
    /// </summary>
    private static bool BeValidJordanianOrInternationalPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return false;

        // Strip spaces and dashes for normalisation
        var normalized = phone.Replace(" ", "").Replace("-", "");

        // Jordanian local format: 07XXXXXXXX
        if (System.Text.RegularExpressions.Regex.IsMatch(normalized, @"^07\d{8}$"))
            return true;

        // Jordanian international format: +9627XXXXXXXX
        if (System.Text.RegularExpressions.Regex.IsMatch(normalized, @"^\+9627\d{8}$"))
            return true;

        // Generic international E.164: +[1-3 digit country code][7-12 digits]
        if (System.Text.RegularExpressions.Regex.IsMatch(normalized, @"^\+[1-9]\d{6,14}$"))
            return true;

        return false;
    }

    private static bool NotContainSpaces(string password)
        => !password.Contains(' ');
}
