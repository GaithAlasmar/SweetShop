using MediatR;
using SweetShop.Models;
using SweetShop.Models.Interfaces;

namespace SweetShop.Features.Products.Queries;

// ── Query ─────────────────────────────────────────────────────────────
public record GetProductByIdQuery(int Id) : IRequest<Product?>;

// ── Handler ───────────────────────────────────────────────────────────
public class GetProductByIdQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductByIdQuery, Product?>
{
    public Task<Product?> Handle(GetProductByIdQuery request,
                                 CancellationToken cancellationToken)
    {
        var product = productRepository.GetProductById(request.Id);
        return Task.FromResult(product);
    }
}
