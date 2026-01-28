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

    public void CreateOrder(Order order)
    {
        order.OrderPlaced = DateTime.Now;
        var shoppingCartItems = _shoppingCart.ShoppingCartItems;
        order.OrderTotal = _shoppingCart.GetShoppingCartTotal();

        _context.Orders.Add(order);
        _context.SaveChanges(); // Save to generate Order Id

        foreach (var shoppingCartItem in shoppingCartItems)
        {
            var orderDetail = new OrderDetail
            {
                Amount = shoppingCartItem.Amount,
                ProductId = shoppingCartItem.Product.Id,
                OrderId = order.Id,
                Price = shoppingCartItem.Product.Price
            };

            _context.OrderDetails.Add(orderDetail);
        }

        _context.SaveChanges();
    }
}
