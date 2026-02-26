using Microsoft.EntityFrameworkCore;
using SweetShop.Data;
using SweetShop.Models;
using SweetShop.Models.Interfaces;

namespace SweetShop.Models.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ShoppingCart _shoppingCart;

    public OrderRepository(ApplicationDbContext context, ShoppingCart shoppingCart)
    {
        _context = context;
        _shoppingCart = shoppingCart;
    }

    /// <summary>Creates an order using items from the ShoppingCart session.</summary>
    public void CreateOrder(Order order)
    {
        order.OrderPlaced = DateTime.Now;
        var shoppingCartItems = _shoppingCart.ShoppingCartItems;
        order.OrderTotal = _shoppingCart.GetShoppingCartTotal();

        _context.Orders.Add(order);
        _context.SaveChanges();

        foreach (var shoppingCartItem in shoppingCartItems)
        {
            var orderDetail = new OrderDetail
            {
                Amount = shoppingCartItem.Amount,
                ProductId = shoppingCartItem.Product.Id,
                ProductVariantId = shoppingCartItem.ProductVariantId,
                OrderId = order.Id,
                Price = shoppingCartItem.ProductVariant?.Price ?? shoppingCartItem.Product.Price
            };

            _context.OrderDetails.Add(orderDetail);
        }

        _context.SaveChanges();
    }

    /// <summary>
    /// Persists an order that was built manually from a ViewModel (CQRS path).
    /// The order's OrderDetails must already be populated before calling this.
    /// </summary>
    public async Task CreateOrderFromViewModelAsync(Order order)
    {
        order.OrderPlaced = DateTime.Now;
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }
}
