using MediatR;
using SweetShop.Models;
using SweetShop.Models.Interfaces;

namespace SweetShop.Features.ShoppingCart.Commands;

// ── Command ───────────────────────────────────────────────────────────
public record AddToCartCommand(int ProductId) : IRequest<Unit>;

// ── Handler ───────────────────────────────────────────────────────────
public class AddToCartCommandHandler(
    IProductRepository productRepository,
    Models.ShoppingCart shoppingCart)
    : IRequestHandler<AddToCartCommand, Unit>
{
    public Task<Unit> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var product = productRepository.GetAllProducts()
                                       .FirstOrDefault(p => p.Id == request.ProductId);
        if (product != null)
        {
            shoppingCart.AddToCart(product, 1);
        }

        return Task.FromResult(Unit.Value);
    }
}
