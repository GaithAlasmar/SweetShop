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
        IValidator<RegisterViewModel> registerValidator) : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IValidator<RegisterViewModel> _registerValidator = registerValidator;

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [EnableRateLimiting("LoginPolicy")] // Max 5 attempts/min per IP
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "محاولة دخول غير ناجحة");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [EnableRateLimiting("LoginPolicy")] // Protect registration against bot sign-ups
        public async Task<IActionResult> Register(RegisterViewModel model)
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
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            // ASP.NET Identity errors (e.g., password complexity from IdentityOptions)
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }
    }
}
