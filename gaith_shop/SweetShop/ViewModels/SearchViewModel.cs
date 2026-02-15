using SweetShop.Models;

namespace SweetShop.ViewModels;

public class SearchViewModel
{
    public string SearchQuery { get; set; } = string.Empty;
    public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();
    public int ResultCount => Products.Count();
}
