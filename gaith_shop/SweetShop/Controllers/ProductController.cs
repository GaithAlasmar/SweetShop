using Microsoft.AspNetCore.Mvc;
using SweetShop.Models.Interfaces;
using SweetShop.ViewModels;

namespace SweetShop.Controllers;

public class ProductController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public IActionResult List(string category)
    {
        var products = _productRepository.GetAllProducts();
        string currentCategory = "All Sweets";

        if (!string.IsNullOrEmpty(category))
        {
            products = products.Where(p => p.Category.Name == category);
            currentCategory = category;
        }

        var productListViewModel = new ProductListViewModel
        {
            Products = products,
            CurrentCategory = currentCategory
        };

        return View(productListViewModel);
    }

    public IActionResult Details(int id)
    {
        var product = _productRepository.GetProductById(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }
}
