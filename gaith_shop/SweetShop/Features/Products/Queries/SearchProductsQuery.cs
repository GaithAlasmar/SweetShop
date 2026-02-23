using MediatR;
using SweetShop.Models;
using SweetShop.Models.Interfaces;
using SweetShop.ViewModels;

namespace SweetShop.Features.Products.Queries;

// ── Query ─────────────────────────────────────────────────────────────
public record SearchProductsQuery(string Term) : IRequest<SearchViewModel>;

// ── Handler ───────────────────────────────────────────────────────────
public class SearchProductsQueryHandler(IProductRepository productRepository)
    : IRequestHandler<SearchProductsQuery, SearchViewModel>
{
    public Task<SearchViewModel> Handle(SearchProductsQuery request,
                                        CancellationToken cancellationToken)
    {
        IEnumerable<Product> results = string.IsNullOrWhiteSpace(request.Term)
            ? Enumerable.Empty<Product>()
            : productRepository.SearchProducts(request.Term);

        var viewModel = new SearchViewModel
        {
            SearchQuery = request.Term ?? string.Empty,
            Products = results
        };

        return Task.FromResult(viewModel);
    }
}
