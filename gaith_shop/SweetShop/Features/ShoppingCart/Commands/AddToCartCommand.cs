using MediatR;
using SweetShop.Models;
using SweetShop.Models.Interfaces;

namespace SweetShop.Features.ShoppingCart.Commands;

// ── Command ───────────────────────────────────────────────────────────
public record AddToCartCommand(int ProductId, int? VariantId = null) : IRequest<Unit>;

// ── Handler ───────────────────────────────────────────────────────────
public class AddToCartCommandHandler(
    IProductRepository productRepository,
    Models.ShoppingCart shoppingCart)
    : IRequestHandler<AddToCartCommand, Unit>
{
    public Task<Unit> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var product = productRepository.GetProductById(request.ProductId);
        if (product != null)
        {
            shoppingCart.AddToCart(product, 1, request.VariantId);
        }

        return Task.FromResult(Unit.Value);
    }
}
