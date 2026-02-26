using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SweetShop.Models;
using SweetShop.Validators;
using SweetShop.ViewModels;

namespace SweetShop.Controllers
{
    public class AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IValidator<RegisterViewModel> registerValidator,
        SweetShop.Services.IEmailSender emailSender) : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IValidator<RegisterViewModel> _registerValidator = registerValidator;
        private readonly SweetShop.Services.IEmailSender _emailSender = emailSender;

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [EnableRateLimiting("LoginPolicy")] // Max 5 attempts/min per IP
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "محاولة دخول غير ناجحة");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [EnableRateLimiting("LoginPolicy")] // Protect registration against bot sign-ups
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            // ── FluentValidation (runs BEFORE UserManager) ─────────────
            var validation = await _registerValidator.ValidateAsync(model);
            if (!validation.IsValid)
            {
                // Map all failures into ModelState → shown by asp-validation-for
                validation.AddToModelState(ModelState);
                return View(model);
            }

            // ── Create user only if validation passed ──────────────────
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, protocol: Request.Scheme);

                var message = $"<div dir='rtl'>يرجى تأكيد حسابك بالنقر <a href='{callbackUrl}'>هنا</a>.</div>";
                await _emailSender.SendEmailAsync(model.Email, "تأكيد الحساب - SweetShop", message);

                return RedirectToAction("RegisterConfirmation", "Account", new { email = model.Email });
            }

            // ASP.NET Identity errors (e.g., password complexity from IdentityOptions)
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterConfirmation(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"تعذر تحميل المستخدم بالمعرف '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            ViewBag.StatusMessage = result.Succeeded
                ? "شكراً لتأكيد بريدك الإلكتروني. يمكنك الآن تسجيل الدخول."
                : "حدث خطأ أثناء تأكيد البريد الإلكتروني.";

            return View();
        }
    }
}
