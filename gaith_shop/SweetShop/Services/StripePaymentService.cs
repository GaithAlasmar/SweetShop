using Stripe.Checkout;
using SweetShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SweetShop.Services;

public class StripePaymentService : IStripePaymentService
{
    public async Task<string> CreateCheckoutSessionAsync(Order order, string domainUrl)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = ["card"],
            LineItems = [],
            Mode = "payment",
            SuccessUrl = domainUrl + $"/Payment/Success?orderId={order.Id}&session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = domainUrl + $"/Payment/Cancel?orderId={order.Id}",
            ClientReferenceId = order.Id.ToString(),
            CustomerEmail = order.Email,
            Metadata = new Dictionary<string, string>
            {
                { "OrderId", order.Id.ToString() }
            }
        };

        // Add order items to Stripe session
        foreach (var item in order.OrderDetails)
        {
            var sessionLineItem = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(item.Price * 100), // Stripe expects amount in cents
                    Currency = "jod", // or usd
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Product.Name,
                    },
                },
                Quantity = item.Amount,
            };
            options.LineItems.Add(sessionLineItem);
        }

        var service = new SessionService();
        Session session = await service.CreateAsync(options);

        return session.Url;
    }
}
