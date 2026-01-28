using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SweetShop.Models;
using SweetShop.Models.Interfaces;

namespace SweetShop.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public AdminController(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public IActionResult Index()
    {
        var products = _productRepository.GetAllProducts();
        return View(products);
    }

    public IActionResult Add()
    {
        ViewBag.CategoryId = new SelectList(_categoryRepository.GetAllCategories(), "Id", "Name");
        return View();
    }

    [HttpPost]
    public IActionResult Add(Product product)
    {
        if (ModelState.IsValid)
        {
            _productRepository.CreateProduct(product);
            return RedirectToAction(nameof(Index));
        }
        ViewBag.CategoryId = new SelectList(_categoryRepository.GetAllCategories(), "Id", "Name", product.CategoryId);
        return View(product);
    }

    public IActionResult Edit(int id)
    {
        var product = _productRepository.GetProductById(id);
        if (product == null) return NotFound();

        ViewBag.CategoryId = new SelectList(_categoryRepository.GetAllCategories(), "Id", "Name", product.CategoryId);
        return View(product);
    }

    [HttpPost]
    public IActionResult Edit(Product product)
    {
        if (ModelState.IsValid)
        {
            _productRepository.UpdateProduct(product);
            return RedirectToAction(nameof(Index));
        }
        ViewBag.CategoryId = new SelectList(_categoryRepository.GetAllCategories(), "Id", "Name", product.CategoryId);
        return View(product);
    }

    public IActionResult Delete(int id)
    {
        var product = _productRepository.GetProductById(id);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var product = _productRepository.GetProductById(id);
        if (product != null)
        {
             _productRepository.DeleteProduct(product);
        }
        return RedirectToAction(nameof(Index));
    }
}
