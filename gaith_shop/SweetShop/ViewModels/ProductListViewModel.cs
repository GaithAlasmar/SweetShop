using SweetShop.Models;

namespace SweetShop.ViewModels;

public class ProductListViewModel
{
    public IEnumerable<Product> Products { get; set; } = new List<Product>();
    public string CurrentCategory { get; set; } = string.Empty;
}
