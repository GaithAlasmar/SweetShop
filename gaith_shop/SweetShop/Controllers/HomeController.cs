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
        var homeViewModel = new HomeViewModel
        {
            PreferredSweets = preferredProducts,
            Categories = _context.Categories.ToList()
        };
        return View(homeViewModel);
    }
}
