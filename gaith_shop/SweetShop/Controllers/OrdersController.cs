using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SweetShop.Features.Orders.Commands;
using SweetShop.Features.Orders.Queries;
using SweetShop.ViewModels;

namespace SweetShop.Controllers;

/// <summary>
/// Thin controller — all business logic delegated to MediatR handlers.
/// No direct DB access; ApplicationDbContext is NOT injected here.
/// </summary>
public class OrdersController(IMediator mediator) : Controller
{
    // ── Bulk Order (event-based) ───────────────────────────────────────

    // GET: /Orders
    public async Task<IActionResult> Index()
    {
        var viewModel = await mediator.Send(new GetBulkOrderFormQuery());
        return View(viewModel);
    }

    // POST: /Orders/SubmitOrder
    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("OrderPolicy")] // Max 10 bulk order submissions per 5 min per user
    public async Task<IActionResult> SubmitOrder(BulkOrderViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Re-populate categories while preserving user input
            var reloadedModel = await mediator.Send(new GetBulkOrderFormQuery(model));
            return View("Index", reloadedModel);
        }

        var result = await mediator.Send(new SubmitBulkOrderCommand(model));

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            var reloadedModel = await mediator.Send(new GetBulkOrderFormQuery(model));
            return View("Index", reloadedModel);
        }

        TempData["SuccessMessage"] = "تم استلام طلبك بنجاح! سنتواصل معك قريباً.";
        TempData["OrderSummary"] = result.OrderSummary;

        return RedirectToAction(nameof(OrderConfirmation));
    }

    // GET: /Orders/OrderConfirmation
    public IActionResult OrderConfirmation()
    {
        if (TempData["SuccessMessage"] == null)
            return RedirectToAction(nameof(Index));

        ViewBag.SuccessMessage = TempData["SuccessMessage"];
        ViewBag.OrderSummary = TempData["OrderSummary"];

        return View();
    }

    // ── Line-item Order (product search) ──────────────────────────────

    // GET: /Orders/Create
    public IActionResult Create()
    {
        var viewModel = new CreateOrderViewModel
        {
            EventDate = DateTime.Now.AddDays(7)
        };
        return View(viewModel);
    }

    // GET: /Orders/SearchProducts?term=...
    [HttpGet]
    public async Task<IActionResult> SearchProducts(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(Array.Empty<object>());

        // Reuse the shared Products search query
        var searchVm = await mediator.Send(
            new Features.Products.Queries.SearchProductsQuery(term));

        var result = searchVm.Products
            .Take(20)
            .Select(p => new { id = p.Id, text = p.Name, price = p.Price });

        return Json(result);
    }

    // POST: /Orders/SaveOrder
    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("OrderPolicy")] // Max 10 custom orders per 5 min per user
    public async Task<IActionResult> SaveOrder(CreateOrderViewModel model)
    {
        if (!ModelState.IsValid)
            return View("Create", model);

        var result = await mediator.Send(new SaveOrderCommand(model));

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            return View("Create", model);
        }

        TempData["SuccessMessage"] = "تم استلام طلبك بنجاح! سنتواصل معك قريباً.";
        TempData["OrderSummary"] = result.OrderSummary;

        return RedirectToAction(nameof(OrderConfirmation));
    }
}
