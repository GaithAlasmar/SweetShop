using Microsoft.EntityFrameworkCore;
using SweetShop.Data;
using SweetShop.Models;
using SweetShop.Models.Interfaces;


namespace SweetShop.Models.Repositories;

public class ProductRepository(ApplicationDbContext context) : IProductRepository
{
    public IEnumerable<Product> GetAllProducts()
    {
        return context.Products.Include(p => p.Category);
    }

    public IEnumerable<Product> GetAllProductsWithDeleted()
    {
        return context.Products.IgnoreQueryFilters().Include(p => p.Category);
    }

    public IEnumerable<Product> GetPreferredProducts()
    {
        return context.Products.Where(p => p.IsPreferredSweet).Include(p => p.Category);
    }

    public Product? GetProductById(int productId)
    {
        return context.Products
            .IgnoreQueryFilters()
            .Include(p => p.Category)
            .Include(p => p.Configurations)
            .Include(p => p.Reviews)
                .ThenInclude(r => r.User)
            .FirstOrDefault(p => p.Id == productId);
    }

    public IEnumerable<Product> SearchProducts(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return [];
        }

        return context.Products
            .Include(p => p.Category)
            .Where(p =>
                (p.Name.Contains(searchTerm) ||
                 (p.Description != null && p.Description.Contains(searchTerm))) &&
                p.InStock)
            .ToList();
    }

    public void CreateProduct(Product product)
    {
        context.Products.Add(product);
        context.SaveChanges();
    }

    public void UpdateProduct(Product product)
    {
        var existingProduct = context.Products
            .Include(p => p.Configurations)
            .FirstOrDefault(p => p.Id == product.Id);

        if (existingProduct != null)
        {
            // Update scalar properties
            context.Entry(existingProduct).CurrentValues.SetValues(product);

            // Handle Configurations (Variants)
            if (product.Configurations != null)
            {
                var incomingIds = product.Configurations.Select(c => c.Id).ToList();

                // Remove variants not in the incoming list
                var variantsToRemove = existingProduct.Configurations
                    .Where(c => !incomingIds.Contains(c.Id))
                    .ToList();

                foreach (var variant in variantsToRemove)
                {
                    context.Remove(variant);
                }

                // Add or update incoming variants
                foreach (var incomingVariant in product.Configurations)
                {
                    var existingVariant = existingProduct.Configurations
                        .FirstOrDefault(c => c.Id == incomingVariant.Id);

                    if (existingVariant != null)
                    {
                        context.Entry(existingVariant).CurrentValues.SetValues(incomingVariant);
                    }
                    else
                    {
                        existingProduct.Configurations.Add(incomingVariant);
                    }
                }
            }

            context.SaveChanges();
        }
    }

    public void DeleteProduct(Product product)
    {
        product.IsDeleted = true;
        context.Products.Update(product);
        context.SaveChanges();
    }
}
