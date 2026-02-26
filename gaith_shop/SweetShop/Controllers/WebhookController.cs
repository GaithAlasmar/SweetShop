using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using SweetShop.Data;
using SweetShop.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace SweetShop.Controllers;

[Route("Webhook")]
[ApiController] // Useful for APIs, formats validation errors automatically
public class WebhookController(ApplicationDbContext context, ShoppingCart shoppingCart, ILogger<WebhookController> logger, IConfiguration configuration) : ControllerBase
{
    // POST: /Webhook/GatewayCallback
    [HttpPost("GatewayCallback")]
    public async Task<IActionResult> GatewayCallback()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var webhookSecret = configuration.GetSection("Stripe")["WebhookSecret"];
            var stripeSignature = Request.Headers["Stripe-Signature"].FirstOrDefault();

            // Verify signature and construct the event securely
            var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, webhookSecret);

            logger.LogInformation("Webhook handled: {EventType}", stripeEvent.Type);

            // Handle the checkout.session.completed event
            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

                if (session != null && session.Metadata != null && session.Metadata.TryGetValue("OrderId", out string? orderIdStr))
                {
                    if (int.TryParse(orderIdStr, out int orderId))
                    {
                        var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

                        if (order != null && order.PaymentStatus == "Pending")
                        {
                            order.PaymentStatus = "Completed";
                            order.TransactionId = session.PaymentIntentId; // Save Stripe's intent ID
                            order.Status = "مدفوع بالكامل"; // Fully Paid

                            // Try to clear the cart for the user safely
                            // Note: We might need Session management improvements for distributed carts
                            shoppingCart.ClearCart();

                            await context.SaveChangesAsync();
                            logger.LogInformation("Order {OrderId} fulfilled successfully via Stripe Webhook.", orderId);
                        }
                    }
                }
            }

            return Ok(); // Acknowledge receipt to Stripe (MUST return 200 OK)
        }
        catch (StripeException e)
        {
            logger.LogError(e, "Stripe signature verification failed.");
            return BadRequest();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing Stripe Webhook.");
            return StatusCode(500);
        }
    }
}
