using MediatR;
using SweetShop.Models;
using SweetShop.Data;

namespace SweetShop.Features.Products.Commands;

public record AddProductReviewCommand(int ProductId, string UserId, int Rating, string? Comment) : IRequest<bool>;

public class AddProductReviewCommandHandler(ApplicationDbContext context) : IRequestHandler<AddProductReviewCommand, bool>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<bool> Handle(AddProductReviewCommand request, CancellationToken cancellationToken)
    {
        var review = new ProductReview
        {
            ProductId = request.ProductId,
            UserId = request.UserId,
            Rating = request.Rating,
            Comment = request.Comment,
            IsApproved = true // Automatically approved for now, can be changed based on business rules
        };

        _context.ProductReviews.Add(review);
        var result = await _context.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}
