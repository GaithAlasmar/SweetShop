using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SweetShop.Models;
using System.ComponentModel.DataAnnotations;

namespace SweetShop.Areas.Identity.Pages.Account.Manage;

public class ChangePasswordModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<ChangePasswordModel> _logger;

    public ChangePasswordModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<ChangePasswordModel> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    [TempData]
    public string? StatusMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "كلمة المرور الحالية مطلوبة")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور الحالية")]
        public string OldPassword { get; set; } = default!;

        [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
        [StringLength(100, ErrorMessage = "يجب أن تكون {0} على الأقل {2} وبحد أقصى {1} حرفًا.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور الجديدة")]
        public string NewPassword { get; set; } = default!;

        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة المرور الجديدة")]
        [Compare("NewPassword", ErrorMessage = "كلمة المرور الجديدة وتأكيد كلمة المرور غير متطابقين.")]
        public string ConfirmPassword { get; set; } = default!;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"غير قادر على تحميل المستخدم بالمعرف '{_userManager.GetUserId(User)}'.");
        }

        var hasPassword = await _userManager.HasPasswordAsync(user);
        if (!hasPassword)
        {
            return RedirectToPage("./SetPassword");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"غير قادر على تحميل المستخدم بالمعرف '{_userManager.GetUserId(User)}'.");
        }

        var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            foreach (var error in changePasswordResult.Errors)
            {
                var errorMessage = error.Code switch
                {
                    "PasswordMismatch" => "كلمة المرور الحالية غير صحيحة.",
                    "PasswordTooShort" => "كلمة المرور قصيرة جدًا.",
                    _ => error.Description
                };
                ModelState.AddModelError(string.Empty, errorMessage);
            }
            return Page();
        }

        await _signInManager.RefreshSignInAsync(user);
        _logger.LogInformation("المستخدم قام بتغيير كلمة المرور بنجاح.");
        StatusMessage = "تم تغيير كلمة المرور الخاصة بك.";

        return RedirectToPage();
    }
}
