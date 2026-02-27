using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetShop.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using SweetShop.Models;

namespace SweetShop.Controllers;

public class PaymentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SweetShop.Services.IStripePaymentService _stripePaymentService;
    private readonly ShoppingCart _shoppingCart;

    public PaymentController(
        ApplicationDbContext context,
        IHttpClientFactory httpClientFactory,
        SweetShop.Services.IStripePaymentService stripePaymentService,
        ShoppingCart shoppingCart)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _stripePaymentService = stripePaymentService;
        _shoppingCart = shoppingCart;
    }

    // Displays the Mock Gateway Interface
    [HttpGet]
    public async Task<IActionResult> Index(int orderId)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null || order.PaymentStatus != "Pending")
        {
            return RedirectToAction("Index", "Home");
        }

        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> ProcessPayment(int orderId, string cvv)
    {
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) return NotFound();

        // Simulate processing time
        await Task.Delay(1500);

        if (cvv == "000")
        {
            // Simulate failure
            order.PaymentStatus = "Failed";
            order.Status = "فشل الدفع";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Cancel), new { orderId = order.Id });
        }
        else
        {
            // Simulate success
            order.PaymentStatus = "Completed";
            order.Status = "قيد التحضير"; // Preparing
            order.TransactionId = "mock_txn_" + Guid.NewGuid().ToString("N").Substring(0, 10);
            await _context.SaveChangesAsync();

            // Clear the shopping cart
            _shoppingCart.ClearCart();

            return RedirectToAction(nameof(Success), new { orderId = order.Id });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Success(int orderId)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null) return NotFound();

        // User sees "Pending" or "Completed" depending on how fast the webhook fired!
        return View(order);
    }

    [HttpGet]
    public async Task<IActionResult> Cancel(int orderId)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null) return NotFound();

        return View(order);
    }
}
