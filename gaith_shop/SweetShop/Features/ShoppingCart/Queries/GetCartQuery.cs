using MediatR;
using SweetShop.Models;
using SweetShop.ViewModels;

namespace SweetShop.Features.ShoppingCart.Queries;

// ── Query ─────────────────────────────────────────────────────────────
public record GetCartQuery : IRequest<ShoppingCartViewModel>;

// ── Handler ───────────────────────────────────────────────────────────
public class GetCartQueryHandler(Models.ShoppingCart shoppingCart)
    : IRequestHandler<GetCartQuery, ShoppingCartViewModel>
{
    public Task<ShoppingCartViewModel> Handle(GetCartQuery request,
                                              CancellationToken cancellationToken)
    {
        var items = shoppingCart.GetShoppingCartItems();
        shoppingCart.ShoppingCartItems = items;

        var viewModel = new ShoppingCartViewModel
        {
            ShoppingCart = shoppingCart,
            ShoppingCartTotal = shoppingCart.GetShoppingCartTotal()
        };

        return Task.FromResult(viewModel);
    }
}
