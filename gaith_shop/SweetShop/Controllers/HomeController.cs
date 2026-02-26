using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using SweetShop.Data;
using SweetShop.Models.Interfaces;
using SweetShop.ViewModels;

namespace SweetShop.Controllers;

public class HomeController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ApplicationDbContext _context;

    public HomeController(IProductRepository productRepository, ApplicationDbContext context)
    {
        _productRepository = productRepository;
        _context = context;
    }

    public IActionResult Index()
    {
        var preferredProducts = _productRepository.GetPreferredProducts();
        var iceCreamFavorites = _productRepository.GetAllProducts()
            .Where(p => p.Category.Name == "Ice Cream" && p.IsPreferredSweet);

        var homeViewModel = new HomeViewModel
        {
            PreferredSweets = preferredProducts,
            IceCreamFavorites = iceCreamFavorites,
            Categories = _context.Categories.ToList()
        };
        return View(homeViewModel);
    }

    public IActionResult Branches()
    {
        var branches = new List<BranchViewModel>
        {
            new()
            {
                Name = "فرع عمّان - الدوار السابع",
                Address = "عمّان، الدوار السابع، شارع عبدالله غوشة، مجمع السابع التجاري",
                PhoneNumber = "+962 6 581 1234",
                WorkingHours = "9:00 صباحاً - 11:00 مساءً",
                GoogleMapsDirectionUrl = "https://maps.google.com/?q=31.9539,35.8753",
                PhotoUrl = "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=600&h=400&fit=crop"
            },
            new()
            {
                Name = "فرع الزرقاء - السوق الجديد",
                Address = "الزرقاء، السوق الجديد، شارع الملك عبدالله الثاني، بجانب البنك العربي",
                PhoneNumber = "+962 5 386 5678",
                WorkingHours = "8:30 صباحاً - 10:30 مساءً",
                GoogleMapsDirectionUrl = "https://maps.google.com/?q=32.0728,36.0880",
                PhotoUrl = "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=600&h=400&fit=crop"
            },
            new()
            {
                Name = "فرع إربد - شارع الجامعة",
                Address = "إربد، شارع الجامعة، مقابل بوابة جامعة اليرموك الشمالية",
                PhoneNumber = "+962 2 721 9012",
                WorkingHours = "9:00 صباحاً - 11:30 مساءً",
                GoogleMapsDirectionUrl = "https://maps.google.com/?q=32.5568,35.8469",
                PhotoUrl = "https://images.unsplash.com/photo-1559329007-40df8a9345d8?w=600&h=400&fit=crop"
            }
        };

        return View(branches);
    }

    // ── Language Switcher ─────────────────────────────────────────────────────
    /// <summary>
    /// Persists the selected culture in a cookie so ASP.NET's
    /// RequestLocalizationMiddleware (CookieRequestCultureProvider) applies
    /// the correct CultureInfo to every subsequent server-rendered request.
    /// </summary>
    /// <param name="lang">Culture code: "ar" or "en"</param>
    /// <param name="returnUrl">URL to redirect back to after setting the cookie.</param>
    [HttpGet]
    public IActionResult SetLanguage(string lang, string returnUrl)
    {
        // Validate lang to prevent open-redirect abuse
        var supported = new[] { "ar", "en" };
        if (!supported.Contains(lang)) lang = "ar";

        // Write the standard ASP.NET culture cookie
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)),
            new CookieOptions
            {
                Expires  = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            }
        );

        // Redirect safely — only allow relative URLs
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }
}
