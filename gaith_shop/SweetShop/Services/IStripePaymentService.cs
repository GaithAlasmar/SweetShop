using SweetShop.Models;

namespace SweetShop.Services;

public interface IStripePaymentService
{
    /// <summary>
    /// Creates a Stripe Checkout Session for the given order and returns the Session URL.
    /// </summary>
    Task<string> CreateCheckoutSessionAsync(Order order, string domainUrl);
}
