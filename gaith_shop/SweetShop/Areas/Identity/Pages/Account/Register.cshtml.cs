using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SweetShop.Models;
using System.ComponentModel.DataAnnotations;

namespace SweetShop.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<RegisterModel> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public string? ReturnUrl { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; } = default!;

    public class InputModel
    {
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [StringLength(100, ErrorMessage = "يجب أن تكون {0} على الأقل {2} وبحد أقصى {1} حرفًا.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; } = default!;

        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة المرور")]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيد كلمة المرور غير متطابقين.")]
        public string ConfirmPassword { get; set; } = default!;
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (ModelState.IsValid)
        {
            var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("المستخدم قام بإنشاء حساب جديد بكلمة مرور.");

                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                // Translate common error messages
                var errorMessage = error.Code switch
                {
                    "DuplicateUserName" => "اسم المستخدم مستخدم بالفعل.",
                    "DuplicateEmail" => "البريد الإلكتروني مستخدم بالفعل.",
                    "InvalidEmail" => "البريد الإلكتروني غير صالح.",
                    "PasswordTooShort" => "كلمة المرور قصيرة جدًا.",
                    "PasswordRequiresNonAlphanumeric" => "يجب أن تحتوي كلمة المرور على حرف خاص واحد على الأقل.",
                    "PasswordRequiresDigit" => "يجب أن تحتوي كلمة المرور على رقم واحد على الأقل ('0'-'9').",
                    "PasswordRequiresUpper" => "يجب أن تحتوي كلمة المرور على حرف كبير واحد على الأقل ('A'-'Z').",
                    "PasswordRequiresLower" => "يجب أن تحتوي كلمة المرور على حرف صغير واحد على الأقل ('a'-'z').",
                    _ => error.Description
                };
                ModelState.AddModelError(string.Empty, errorMessage);
            }
        }

        return Page();
    }
}
