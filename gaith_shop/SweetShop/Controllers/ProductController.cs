using MediatR;
using Microsoft.AspNetCore.Mvc;
using SweetShop.Features.Products.Queries;

namespace SweetShop.Controllers;

/// <summary>
/// Thin controller — dispatches Queries/Commands to MediatR handlers.
/// No business logic lives here.
/// </summary>
public class ProductController(IMediator mediator) : Controller
{
    // GET: /Product/List?category=كنافة
    public async Task<IActionResult> List(string category)
    {
        var viewModel = await mediator.Send(new GetAllProductsQuery(category));
        return View(viewModel);
    }

    // GET: /Product/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var product = await mediator.Send(new GetProductByIdQuery(id));
        if (product == null)
            return NotFound();

        return View(product);
    }

    // GET: /Product/Search?q=كنافة
    public async Task<IActionResult> Search(string q)
    {
        var viewModel = await mediator.Send(new SearchProductsQuery(q));
        return View(viewModel);
    }
}
