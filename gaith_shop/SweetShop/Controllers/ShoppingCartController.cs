using MediatR;
using Microsoft.AspNetCore.Mvc;
using SweetShop.Features.ShoppingCart.Commands;
using SweetShop.Features.ShoppingCart.Queries;

namespace SweetShop.Controllers;

/// <summary>
/// Thin controller — dispatches all cart operations to MediatR handlers.
/// No repository or ShoppingCart injected directly.
/// </summary>
public class ShoppingCartController(IMediator mediator) : Controller
{
    // GET: /ShoppingCart
    public async Task<IActionResult> Index()
    {
        var viewModel = await mediator.Send(new GetCartQuery());
        return View(viewModel);
    }

    // GET: /ShoppingCart/AddToShoppingCart/5?variantId=10
    public async Task<IActionResult> AddToShoppingCart(int productId, int? variantId)
    {
        await mediator.Send(new AddToCartCommand(productId, variantId));
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Ok();
        return RedirectToAction("Index");
    }

    // GET: /ShoppingCart/RemoveFromShoppingCart/5?variantId=10
    public async Task<IActionResult> RemoveFromShoppingCart(int productId, int? variantId)
    {
        await mediator.Send(new RemoveFromCartCommand(productId, variantId));
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Ok();
        return RedirectToAction("Index");
    }

    // GET: /ShoppingCart/IncreaseQuantity/5?variantId=10
    public async Task<IActionResult> IncreaseQuantity(int productId, int? variantId)
    {
        await mediator.Send(new ChangeCartQuantityCommand(productId, QuantityChange.Increase, variantId));
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Ok();
        return RedirectToAction("Index");
    }

    // GET: /ShoppingCart/DecreaseQuantity/5?variantId=10
    public async Task<IActionResult> DecreaseQuantity(int productId, int? variantId)
    {
        await mediator.Send(new ChangeCartQuantityCommand(productId, QuantityChange.Decrease, variantId));
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Ok();
        return RedirectToAction("Index");
    }

    // GET: /ShoppingCart/GetCartPartial
    public async Task<IActionResult> GetCartPartial()
    {
        var viewModel = await mediator.Send(new GetCartQuery());
        return PartialView("_CartPartial", viewModel);
    }

    // GET: /ShoppingCart/GetCartCount
    public async Task<IActionResult> GetCartCount()
    {
        var count = await mediator.Send(new GetCartCountQuery());
        return Ok(count);
    }
}
