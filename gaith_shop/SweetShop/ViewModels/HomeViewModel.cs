using SweetShop.Models;

namespace SweetShop.ViewModels;

public class HomeViewModel
{
    public IEnumerable<Product> PreferredSweets { get; set; } = new List<Product>();
}
