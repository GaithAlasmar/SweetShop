using SweetShop.Data;
using SweetShop.Models;
using SweetShop.Models.Interfaces;


namespace SweetShop.Models.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Category> GetAllCategories()
    {
        return _context.Categories;
    }
}
