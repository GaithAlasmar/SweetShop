using MediatR;
using SweetShop.Features.Shared;
using SweetShop.Models;
using SweetShop.Models.Interfaces;
using SweetShop.ViewModels;

namespace SweetShop.Features.Orders.Commands;

// ── Command ───────────────────────────────────────────────────────────
/// <summary>
/// Creates a detailed order from a CreateOrderViewModel (line-item order).
/// Looks up product prices, builds OrderDetails, and persists via IOrderRepository.
/// </summary>
public record SaveOrderCommand(CreateOrderViewModel Model) : IRequest<SaveOrderResult>;

public record SaveOrderResult(bool Success, string? OrderSummary, string? ErrorMessage = null);

// ── Handler ───────────────────────────────────────────────────────────
public class SaveOrderCommandHandler(
    IOrderRepository orderRepository,
    IProductRepository productRepository)
    : IRequestHandler<SaveOrderCommand, SaveOrderResult>
{
    public async Task<SaveOrderResult> Handle(SaveOrderCommand request,
                                              CancellationToken cancellationToken)
    {
        var model = request.Model;

        if (model.Items == null || model.Items.Count == 0)
        {
            return new SaveOrderResult(false, null, "الرجاء إضافة منتج واحد على الأقل");
        }

        var order = new Order
        {
            FirstName = model.CustomerName,
            LastName = string.Empty,
            PhoneNumber = model.PhoneNumber,
            Email = string.Empty,
            AddressLine1 = string.Empty,
            ZipCode = string.Empty,
            City = string.Empty,
            Country = string.Empty,
            OrderTotal = 0
        };

        foreach (var item in model.Items)
        {
            var product = productRepository.GetProductById(item.ProductId);
            if (product == null) continue;

            order.OrderDetails.Add(new OrderDetail
            {
                ProductId = item.ProductId,
                Amount = (int)Math.Ceiling(item.Quantity),
                Price = product.Price * item.Quantity
            });

            order.OrderTotal += product.Price * item.Quantity;
        }

        await orderRepository.CreateOrderFromViewModelAsync(order);

        var orderSummary = $"طلبية جديدة من: {model.CustomerName}\n" +
                          $"الهاتف: {model.PhoneNumber}\n" +
                          $"تاريخ المناسبة: {model.EventDate:yyyy-MM-dd}\n" +
                          $"الملاحظات: {model.AdditionalNotes}\n" +
                          $"المنتجات المطلوبة:\n" +
                          string.Join("\n", model.Items.Select(i =>
                              $"- {i.ProductName}: {i.Quantity} {i.Unit}"));

        return new SaveOrderResult(true, orderSummary);
    }
}
