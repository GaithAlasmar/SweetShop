using System.ComponentModel.DataAnnotations;

namespace SweetShop.Models;

public class ShoppingCartItem
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public int? ProductVariantId { get; set; }
    public ProductVariant? ProductVariant { get; set; }
    public int Amount { get; set; }

    public string? ShoppingCartId { get; set; }
}
