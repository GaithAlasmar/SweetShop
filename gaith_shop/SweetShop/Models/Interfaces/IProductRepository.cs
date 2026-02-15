using SweetShop.Models;

namespace SweetShop.Models.Interfaces;

public interface IProductRepository
{
    IEnumerable<Product> GetAllProducts();
    IEnumerable<Product> GetPreferredProducts();
    Product? GetProductById(int productId);
    IEnumerable<Product> SearchProducts(string searchTerm);
    void CreateProduct(Product product);
    void UpdateProduct(Product product);
    void DeleteProduct(Product product);
}
