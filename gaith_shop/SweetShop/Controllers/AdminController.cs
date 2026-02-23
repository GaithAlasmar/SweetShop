using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SweetShop.Data;
using SweetShop.Models;
using SweetShop.Models.Interfaces;

namespace SweetShop.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ApplicationDbContext _context;

    public AdminController(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ApplicationDbContext context)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _context = context;
    }

    // ─── Products ────────────────────────────────────────────────────────────

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
    [ValidateAntiForgeryToken]
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
    [ValidateAntiForgeryToken]
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
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var product = _productRepository.GetProductById(id);
        if (product != null)
        {
            _productRepository.DeleteProduct(product);
        }
        return RedirectToAction(nameof(Index));
    }

    // ─── Orders ──────────────────────────────────────────────────────────────

    public async Task<IActionResult> Orders()
    {
        var orders = await _context.Orders
            .OrderByDescending(o => o.OrderPlaced)
            .ToListAsync();
        return View(orders);
    }

    public async Task<IActionResult> OrderDetails(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();
        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrderStatus(int id, string status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order != null)
        {
            order.Status = status;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Orders));
    }

    // ─── Settings ────────────────────────────────────────────────────────────

    public async Task<IActionResult> Settings()
    {
        var settings = await _context.SiteSettings.FirstOrDefaultAsync()
                       ?? new SiteSettings();
        return View(settings);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Settings(SiteSettings model)
    {
        if (!ModelState.IsValid) return View(model);

        var existing = await _context.SiteSettings.FirstOrDefaultAsync();
        if (existing == null)
        {
            _context.SiteSettings.Add(model);
        }
        else
        {
            existing.SiteName = model.SiteName;
            existing.WelcomeMessage = model.WelcomeMessage;
            existing.ContactPhone = model.ContactPhone;
            existing.ContactEmail = model.ContactEmail;
            existing.Address = model.Address;
            existing.FooterMessage = model.FooterMessage;
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "✅ تم حفظ الإعدادات بنجاح!";
        return RedirectToAction(nameof(Settings));
    }
}
