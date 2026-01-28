using System.ComponentModel.DataAnnotations;

namespace SweetShop.Models;

public class ShoppingCartItem
{
    public int Id { get; set; }

    public Product Product { get; set; } = default!;
    public int Amount { get; set; }

    public string? ShoppingCartId { get; set; }
}
