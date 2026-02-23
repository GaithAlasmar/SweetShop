using MediatR;

namespace SweetShop.Features.ShoppingCart.Queries;

// ── Query ─────────────────────────────────────────────────────────────
public record GetCartCountQuery : IRequest<int>;

// ── Handler ───────────────────────────────────────────────────────────
public class GetCartCountQueryHandler(Models.ShoppingCart shoppingCart)
    : IRequestHandler<GetCartCountQuery, int>
{
    public Task<int> Handle(GetCartCountQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(shoppingCart.GetShoppingCartTotalCount());
    }
}
