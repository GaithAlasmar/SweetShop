using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SweetShop.Features.Products.Queries;
using SweetShop.Features.Products.Commands;

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

        var approvedReviews = product.Reviews.Where(r => r.IsApproved).OrderByDescending(r => r.CreatedAt).ToList();

        var viewModel = new SweetShop.ViewModels.ProductDetailsViewModel
        {
            ProductId = product.Id,
            Name = product.Name,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            BasePrice = product.Price, // Keeping the old Price mapping here, assuming it acts as BasePrice
            WeightOptions = [.. product.Configurations
                .Where(v => v.InStock)
                .Select(v => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = $"{v.Weight} - {v.Price:C}",
                    Value = v.Id.ToString(),
                    Selected = v.IsDefault
                })],
            Reviews = approvedReviews,
            TotalReviews = approvedReviews.Count,
            AverageRating = approvedReviews.Count > 0 ? approvedReviews.Average(r => r.Rating) : 0
        };

        return View(viewModel);
    }

    // GET: /Product/Search?q=كنافة
    public async Task<IActionResult> Search(string q)
    {
        var viewModel = await mediator.Send(new SearchProductsQuery(q));
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReview(SweetShop.ViewModels.ProductDetailsViewModel model)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return RedirectToAction("Login", "Account");
        }

        if (model.NewReviewRating < 1 || model.NewReviewRating > 5)
        {
            // Invalid rating
            return RedirectToAction("Details", new { id = model.ProductId });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId != null)
        {
            await mediator.Send(new AddProductReviewCommand(model.ProductId, userId, model.NewReviewRating, model.NewReviewComment));
        }

        return RedirectToAction("Details", new { id = model.ProductId });
    }
}
