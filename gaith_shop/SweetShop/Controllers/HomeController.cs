using Microsoft.AspNetCore.Mvc;
using SweetShop.Models.Interfaces;
using SweetShop.ViewModels;

namespace SweetShop.Controllers;

public class HomeController : Controller
{
    private readonly IProductRepository _productRepository;

    public HomeController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public IActionResult Index()
    {
        var preferredProducts = _productRepository.GetPreferredProducts();
        var homeViewModel = new HomeViewModel
        {
            PreferredSweets = preferredProducts
        };
        return View(homeViewModel);
    }
}
