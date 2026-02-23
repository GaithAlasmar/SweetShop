using SweetShop.Models;

namespace SweetShop.Models.Interfaces;

public interface IOrderRepository
{
    void CreateOrder(Order order);

    /// <summary>
    /// Persists an order that was built manually (not from ShoppingCart).
    /// Used by the CQRS SaveOrderCommandHandler.
    /// </summary>
    Task CreateOrderFromViewModelAsync(Order order);
}
