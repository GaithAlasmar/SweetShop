using MediatR;
using SweetShop.Models;
using SweetShop.Models.Interfaces;
using SweetShop.ViewModels;

namespace SweetShop.Features.Products.Queries;

// ── Query ─────────────────────────────────────────────────────────────
/// <summary>Returns a filtered product list. Pass null Category for all products.</summary>
public record GetAllProductsQuery(string? Category) : IRequest<ProductListViewModel>;

// ── Handler ───────────────────────────────────────────────────────────
public class GetAllProductsQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetAllProductsQuery, ProductListViewModel>
{
    public Task<ProductListViewModel> Handle(GetAllProductsQuery request,
                                             CancellationToken cancellationToken)
    {
        IEnumerable<Product> products = productRepository.GetAllProducts();
        string currentCategory = "جميع الحلويات";

        if (!string.IsNullOrEmpty(request.Category))
        {
            products = products
                .Where(p => p.Category.Name == request.Category && p.InStock);
            currentCategory = request.Category;
        }
        else
        {
            products = products.Where(p => p.InStock);
        }

        var viewModel = new ProductListViewModel
        {
            Products = products.ToList(),
            CurrentCategory = currentCategory
        };

        return Task.FromResult(viewModel);
    }
}
