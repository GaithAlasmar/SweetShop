using Microsoft.EntityFrameworkCore;
using SweetShop.Data;
using SweetShop.Models;
using SweetShop.Models.Interfaces;


namespace SweetShop.Models.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Product> GetAllProducts()
    {
        return _context.Products.Include(p => p.Category);
    }

    public IEnumerable<Product> GetPreferredProducts()
    {
        return _context.Products.Where(p => p.IsPreferredSweet).Include(p => p.Category);
    }

    public Product? GetProductById(int productId)
    {
        return _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == productId);
    }
    
    public void CreateProduct(Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();
    }
    
    public void UpdateProduct(Product product)
    {
        _context.Products.Update(product);
        _context.SaveChanges();
    }
    
    public void DeleteProduct(Product product)
    {
        _context.Products.Remove(product);
        _context.SaveChanges();
    }
}
