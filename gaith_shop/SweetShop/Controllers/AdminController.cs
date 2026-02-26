using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SweetShop.Data;
using SweetShop.Models;
using SweetShop.Models.Interfaces;
using SweetShop.Services;
using System.Text;

namespace SweetShop.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    ApplicationDbContext context,
    IImageService imageService,
    IEmailSender emailSender) : Controller
{




    // ─── Dashboard ───────────────────────────────────────────────────────────

    public IActionResult Dashboard()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetMonthlySalesData()
    {
        // Fetch only shipped/delivered orders to calculate true sales
        var salesData = await context.Orders
            .Where(o => (o.Status == "Shipped" || o.Status == "Delivered") && !o.IsDeleted)
            .GroupBy(o => new { o.OrderPlaced.Year, o.OrderPlaced.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                TotalSales = g.Sum(o => o.OrderTotal)
            })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToListAsync();

        return Json(salesData);
    }

    // ─── Products ────────────────────────────────────────────────────────────

    public IActionResult Index()
    {
        var products = productRepository.GetAllProductsWithDeleted();
        return View(products);
    }

    public IActionResult Add()
    {
        ViewBag.CategoryId = new SelectList(categoryRepository.GetAllCategories(), "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(Product product, IFormFile imageFile)
    {
        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                string cloudUrl = await imageService.UploadImageAsync(imageFile, "products");
                product.ImageUrl = cloudUrl;
            }

            productRepository.CreateProduct(product);
            return RedirectToAction(nameof(Index));
        }
        ViewBag.CategoryId = new SelectList(categoryRepository.GetAllCategories(), "Id", "Name", product.CategoryId);
        return View(product);
    }

    public IActionResult Edit(int id)
    {
        var product = productRepository.GetProductById(id);
        if (product == null) return NotFound();

        ViewBag.CategoryId = new SelectList(categoryRepository.GetAllCategories(), "Id", "Name", product.CategoryId);
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Product product, IFormFile imageFile)
    {
        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                // Delete old image if one existed
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    await imageService.DeleteImageAsync(product.ImageUrl);
                }

                string cloudUrl = await imageService.UploadImageAsync(imageFile, "products");
                product.ImageUrl = cloudUrl;
            }

            // Instead of just calling repository directly which might have issues with nested collections, 
            // since we do a postback with everything, EF will either update or track them if we do it right in the repo.
            productRepository.UpdateProduct(product);
            return RedirectToAction(nameof(Index));
        }
        ViewBag.CategoryId = new SelectList(categoryRepository.GetAllCategories(), "Id", "Name", product.CategoryId);
        return View(product);
    }

    public IActionResult Delete(int id)
    {
        var product = productRepository.GetProductById(id);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = productRepository.GetProductById(id);
        if (product != null)
        {
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                await imageService.DeleteImageAsync(product.ImageUrl);
            }
            productRepository.DeleteProduct(product);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Restore(int id)
    {
        var product = productRepository.GetProductById(id);
        if (product != null)
        {
            product.IsDeleted = false;
            productRepository.UpdateProduct(product);
        }
        return RedirectToAction(nameof(Index));
    }

    // ─── Orders ──────────────────────────────────────────────────────────────

    public async Task<IActionResult> Orders()
    {
        var orders = await context.Orders
            .OrderByDescending(o => o.OrderPlaced)
            .ToListAsync();
        return View(orders);
    }

    public async Task<IActionResult> OrderDetails(int id)
    {
        var order = await context.Orders
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
        var order = await context.Orders
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.ProductVariant)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order != null)
        {
            order.Status = status;
            await context.SaveChangesAsync();

            // Send email if order is processing
            if (status == "Processing")
            {
                await SendInvoiceEmailAsync(order);
            }
        }
        return RedirectToAction(nameof(Orders));
    }

    private async Task SendInvoiceEmailAsync(Order order)
    {
        var subject = $"SweetShop - تأكيد الطلب رقم #{order.Id}";

        var sb = new StringBuilder();
        sb.Append("<div dir='rtl' style='font-family: Arial, sans-serif; color: #333;'>");
        sb.Append($"<h2>مرحباً {order.FirstName} {order.LastName}،</h2>");
        sb.Append("<p>نود إعلامك بأنه قد تمت الموافقة على طلبك بنجاح، وهو الآن قيد التجهيز.</p>");
        sb.Append("<h3>تفاصيل الفاتورة:</h3>");

        sb.Append("<table style='width: 100%; border-collapse: collapse; margin-bottom: 20px;'>");
        sb.Append("<thead>");
        sb.Append("<tr style='background-color: #f8f9fa;'>");
        sb.Append("<th style='border: 1px solid #ddd; padding: 8px; text-align: right;'>المنتج</th>");
        sb.Append("<th style='border: 1px solid #ddd; padding: 8px; text-align: center;'>الكمية</th>");
        sb.Append("<th style='border: 1px solid #ddd; padding: 8px; text-align: left;'>السعر</th>");
        sb.Append("</tr>");
        sb.Append("</thead>");
        sb.Append("<tbody>");

        foreach (var item in order.OrderDetails)
        {
            string variantDetails = item.ProductVariant != null ? $" ({item.ProductVariant.Weight})" : "";
            sb.Append("<tr>");
            sb.Append($"<td style='border: 1px solid #ddd; padding: 8px;'>{item.Product.Name}{variantDetails}</td>");
            sb.Append($"<td style='border: 1px solid #ddd; padding: 8px; text-align: center;'>{item.Amount}</td>");
            sb.Append($"<td style='border: 1px solid #ddd; padding: 8px; text-align: left;'>{item.Price:c}</td>");
            sb.Append("</tr>");
        }

        sb.Append("</tbody>");
        sb.Append("<tfoot>");
        sb.Append("<tr>");
        sb.Append("<td colspan='2' style='border: 1px solid #ddd; padding: 8px; font-weight: bold; text-align: right;'>الإجمالي</td>");
        sb.Append($"<td style='border: 1px solid #ddd; padding: 8px; font-weight: bold; text-align: left;'>{order.OrderTotal:c}</td>");
        sb.Append("</tr>");
        sb.Append("</tfoot>");
        sb.Append("</table>");

        sb.Append($"<p><strong>طريقة الدفع:</strong> {order.PaymentStatus}</p>");
        sb.Append($"<p><strong>عنوان التوصيل:</strong> {order.AddressLine1}, {order.City}</p>");
        sb.Append("<p>شكراً لتسوقك معنا!</p>");
        sb.Append("</div>");

        try
        {
            await emailSender.SendEmailAsync(order.Email, subject, sb.ToString());
        }
        catch (Exception ex)
        {
            // Log exception here in a real app, silently fail to not interrupt the order update response for the admin
            Console.WriteLine($"Failed to send email: {ex.Message}");
        }
    }

    // ─── Settings ────────────────────────────────────────────────────────────

    public async Task<IActionResult> Settings()
    {
        var settings = await context.SiteSettings.FirstOrDefaultAsync()
                       ?? new SiteSettings();
        return View(settings);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Settings(SiteSettings model)
    {
        if (!ModelState.IsValid) return View(model);

        var existing = await context.SiteSettings.FirstOrDefaultAsync();
        if (existing == null)
        {
            context.SiteSettings.Add(model);
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

        await context.SaveChangesAsync();
        TempData["SuccessMessage"] = "✅ تم حفظ الإعدادات بنجاح!";
        return RedirectToAction(nameof(Settings));
    }
}
