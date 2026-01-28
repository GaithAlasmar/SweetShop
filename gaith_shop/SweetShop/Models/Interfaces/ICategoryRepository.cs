using SweetShop.Models;

namespace SweetShop.Models.Interfaces;

public interface ICategoryRepository
{
    IEnumerable<Category> GetAllCategories();
}
