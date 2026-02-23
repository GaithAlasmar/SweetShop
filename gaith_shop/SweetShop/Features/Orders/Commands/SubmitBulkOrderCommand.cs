using MediatR;
using SweetShop.Features.Shared;
using SweetShop.ViewModels;

namespace SweetShop.Features.Orders.Commands;

// ── Command ───────────────────────────────────────────────────────────
/// <summary>
/// Validates and processes a Bulk Order (event-based order with category quantities).
/// Returns a CommandResult with success flag and the order summary string for TempData.
/// </summary>
public record SubmitBulkOrderCommand(BulkOrderViewModel Model)
    : IRequest<SubmitBulkOrderResult>;

public record SubmitBulkOrderResult(bool Success, string? OrderSummary, string? ErrorMessage = null);

// ── Handler ───────────────────────────────────────────────────────────
public class SubmitBulkOrderCommandHandler
    : IRequestHandler<SubmitBulkOrderCommand, SubmitBulkOrderResult>
{
    public Task<SubmitBulkOrderResult> Handle(SubmitBulkOrderCommand request,
                                              CancellationToken cancellationToken)
    {
        var model = request.Model;

        // Filter out items with zero quantity
        var orderedItems = model.CategoryItems
                               .Where(ci => ci.Quantity > 0)
                               .ToList();

        if (orderedItems.Count == 0)
        {
            return Task.FromResult(new SubmitBulkOrderResult(
                false, null, "الرجاء تحديد كمية لمنتج واحد على الأقل"));
        }

        // Build summary string (could be persisted to DB in a future iteration)
        var orderSummary = $"طلبية جديدة من: {model.CustomerName}\n" +
                          $"الهاتف: {model.PhoneNumber}\n" +
                          $"تاريخ المناسبة: {model.EventDate:yyyy-MM-dd}\n" +
                          $"الملاحظات: {model.AdditionalNotes}\n" +
                          $"المنتجات المطلوبة:\n" +
                          string.Join("\n", orderedItems.Select(i =>
                              $"- {i.CategoryName}: {i.Quantity} {i.Unit}"));

        return Task.FromResult(new SubmitBulkOrderResult(true, orderSummary));
    }
}
