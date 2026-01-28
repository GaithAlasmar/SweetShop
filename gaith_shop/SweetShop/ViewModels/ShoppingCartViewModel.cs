using SweetShop.Models;

namespace SweetShop.ViewModels;

public class ShoppingCartViewModel
{
    public ShoppingCart ShoppingCart { get; set; } = default!;
    public decimal ShoppingCartTotal { get; set; }
}
