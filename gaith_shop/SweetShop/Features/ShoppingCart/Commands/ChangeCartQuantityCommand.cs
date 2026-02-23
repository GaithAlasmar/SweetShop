using MediatR;
using SweetShop.Models;
using SweetShop.Models.Interfaces;

namespace SweetShop.Features.ShoppingCart.Commands;

// ── Enum ──────────────────────────────────────────────────────────────
public enum QuantityChange { Increase, Decrease }

// ── Command ───────────────────────────────────────────────────────────
public record ChangeCartQuantityCommand(int ProductId, QuantityChange Change) : IRequest<Unit>;

// ── Handler ───────────────────────────────────────────────────────────
public class ChangeCartQuantityCommandHandler(
    IProductRepository productRepository,
    Models.ShoppingCart shoppingCart)
    : IRequestHandler<ChangeCartQuantityCommand, Unit>
{
    public Task<Unit> Handle(ChangeCartQuantityCommand request, CancellationToken cancellationToken)
    {
        var product = productRepository.GetAllProducts()
                                       .FirstOrDefault(p => p.Id == request.ProductId);
        if (product == null)
            return Task.FromResult(Unit.Value);

        if (request.Change == QuantityChange.Increase)
        {
            shoppingCart.AddToCart(product, 1);
        }
        else // Decrease
        {
            // RemoveFromCart decrements by 1 (or removes if qty == 1)
            shoppingCart.RemoveFromCart(product);
        }

        return Task.FromResult(Unit.Value);
    }
}
