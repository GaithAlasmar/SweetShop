using MediatR;
using SweetShop.Models;
using SweetShop.Models.Interfaces;

namespace SweetShop.Features.ShoppingCart.Commands;

// ── Command ───────────────────────────────────────────────────────────
public record RemoveFromCartCommand(int ProductId, int? VariantId = null) : IRequest<Unit>;

// ── Handler ───────────────────────────────────────────────────────────
public class RemoveFromCartCommandHandler(
    IProductRepository productRepository,
    Models.ShoppingCart shoppingCart)
    : IRequestHandler<RemoveFromCartCommand, Unit>
{
    public Task<Unit> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
    {
        var product = productRepository.GetProductById(request.ProductId);
        if (product != null)
        {
            shoppingCart.RemoveTotalFromCart(product, request.VariantId);
        }

        return Task.FromResult(Unit.Value);
    }
}
