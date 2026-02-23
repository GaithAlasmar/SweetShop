using MediatR;
using SweetShop.Models;
using SweetShop.Models.Interfaces;

namespace SweetShop.Features.ShoppingCart.Commands;

// ── Command ───────────────────────────────────────────────────────────
public record RemoveFromCartCommand(int ProductId) : IRequest<Unit>;

// ── Handler ───────────────────────────────────────────────────────────
public class RemoveFromCartCommandHandler(
    IProductRepository productRepository,
    Models.ShoppingCart shoppingCart)
    : IRequestHandler<RemoveFromCartCommand, Unit>
{
    public Task<Unit> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
    {
        var product = productRepository.GetAllProducts()
                                       .FirstOrDefault(p => p.Id == request.ProductId);
        if (product != null)
        {
            shoppingCart.RemoveTotalFromCart(product);
        }

        return Task.FromResult(Unit.Value);
    }
}
