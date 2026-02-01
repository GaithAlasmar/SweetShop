using SweetShop.Models;

namespace SweetShop.ViewModels;

public class HomeViewModel
{
    public IEnumerable<Product> PreferredSweets { get; set; } = new List<Product>();
    public List<Category> Categories { get; set; } = new List<Category>();
}
