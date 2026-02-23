using MediatR;
using SweetShop.Models.Interfaces;
using SweetShop.ViewModels;

namespace SweetShop.Features.Orders.Queries;

// ── Query ─────────────────────────────────────────────────────────────
/// <summary>
/// Loads the Bulk Order form ViewModel, populating categories from the DB.
/// Accepts an optional existing model so we can re-display form on validation failure.
/// </summary>
public record GetBulkOrderFormQuery(BulkOrderViewModel? ExistingModel = null)
    : IRequest<BulkOrderViewModel>;

// ── Handler ───────────────────────────────────────────────────────────
public class GetBulkOrderFormQueryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<GetBulkOrderFormQuery, BulkOrderViewModel>
{
    public Task<BulkOrderViewModel> Handle(GetBulkOrderFormQuery request,
                                           CancellationToken cancellationToken)
    {
        var categories = categoryRepository.GetAllCategories()
                                           .OrderBy(c => c.Name)
                                           .ToList();

        // If we already have an existing model (re-display after failure), preserve
        // the customer's previously entered quantities.
        var existingItems = request.ExistingModel?.CategoryItems ?? [];

        var viewModel = new BulkOrderViewModel
        {
            CustomerName = request.ExistingModel?.CustomerName ?? string.Empty,
            PhoneNumber = request.ExistingModel?.PhoneNumber ?? string.Empty,
            EventDate = request.ExistingModel?.EventDate ?? DateTime.Now.AddDays(7),
            AdditionalNotes = request.ExistingModel?.AdditionalNotes,
            CategoryItems = [..categories.Select(c => new CategoryOrderItem
            {
                CategoryId   = c.Id,
                CategoryName = c.Name,
                Quantity     = existingItems.FirstOrDefault(ci => ci.CategoryId == c.Id)?.Quantity ?? 0,
                Unit         = existingItems.FirstOrDefault(ci => ci.CategoryId == c.Id)?.Unit ?? "كيلو"
            })]
        };

        return Task.FromResult(viewModel);
    }
}
