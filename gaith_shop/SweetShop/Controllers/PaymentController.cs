using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetShop.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace SweetShop.Controllers;

[Authorize]
public class PaymentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SweetShop.Services.IStripePaymentService _stripePaymentService;

    public PaymentController(ApplicationDbContext context, IHttpClientFactory httpClientFactory, SweetShop.Services.IStripePaymentService stripePaymentService)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _stripePaymentService = stripePaymentService;
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

    // Processes the Stripe Payment Checkout
    [HttpPost]
    public async Task<IActionResult> ProcessPayment(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) return NotFound();

        // Pass domain URL (e.g. https://localhost:8080) for success and cancel callbacks
        var domain = $"{Request.Scheme}://{Request.Host}";

        // Generate the Stripe Session URL
        var sessionUrl = await _stripePaymentService.CreateCheckoutSessionAsync(order, domain);

        // Redirect the user to Stripe Secure Checkout
        Response.Headers["Location"] = sessionUrl;
        return new StatusCodeResult(303);
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
