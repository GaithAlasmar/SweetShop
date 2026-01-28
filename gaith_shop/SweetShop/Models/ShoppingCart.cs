using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SweetShop.Data;

namespace SweetShop.Models;

public class ShoppingCart(ApplicationDbContext context)
{
    private readonly ApplicationDbContext _context = context;

    public string ShoppingCartId { get; set; } = default!;
    public List<ShoppingCartItem> ShoppingCartItems { get; set; } = default!;

    public static ShoppingCart GetCart(IServiceProvider services)
    {
        ISession? session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;
        var context = services.GetService<ApplicationDbContext>() ?? throw new Exception("Error retrieving ApplicationDbContext");

        string cartId = session?.GetString("CartId") ?? Guid.NewGuid().ToString();
        session?.SetString("CartId", cartId);

        return new ShoppingCart(context) { ShoppingCartId = cartId };
    }

    public void AddToCart(Product product, int amount)
    {
        var shoppingCartItem = _context.ShoppingCartItems.SingleOrDefault(
            s => s.Product.Id == product.Id && s.ShoppingCartId == ShoppingCartId);

        if (shoppingCartItem == null)
        {
            shoppingCartItem = new ShoppingCartItem
            {
                ShoppingCartId = ShoppingCartId,
                Product = product,
                Amount = amount
            };

            _context.ShoppingCartItems.Add(shoppingCartItem);
        }
        else
        {
            shoppingCartItem.Amount++;
        }
        _context.SaveChanges();
    }

    public int RemoveFromCart(Product product)
    {
        var shoppingCartItem = _context.ShoppingCartItems.SingleOrDefault(
            s => s.Product.Id == product.Id && s.ShoppingCartId == ShoppingCartId);

        var localAmount = 0;

        if (shoppingCartItem != null)
        {
            if (shoppingCartItem.Amount > 1)
            {
                shoppingCartItem.Amount--;
                localAmount = shoppingCartItem.Amount;
            }
            else
            {
                _context.ShoppingCartItems.Remove(shoppingCartItem);
            }
        }

        _context.SaveChanges();

        return localAmount;
    }

    public List<ShoppingCartItem> GetShoppingCartItems()
    {
        return ShoppingCartItems ??=
        [
            .. _context.ShoppingCartItems
                .Where(c => c.ShoppingCartId == ShoppingCartId)
                .Include(s => s.Product)
        ];
    }

    public void ClearCart()
    {
        var cartItems = _context.ShoppingCartItems
            .Where(cart => cart.ShoppingCartId == ShoppingCartId);

        _context.ShoppingCartItems.RemoveRange(cartItems);
        _context.SaveChanges();
    }

    public decimal GetShoppingCartTotal()
    {
        var total = _context.ShoppingCartItems
            .Where(c => c.ShoppingCartId == ShoppingCartId)
            .Select(c => c.Product.Price * c.Amount)
            .Sum();
        return total;
    }
}
