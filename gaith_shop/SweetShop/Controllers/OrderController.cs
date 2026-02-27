using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetShop.Models;
using SweetShop.Models.Interfaces;

namespace SweetShop.Controllers;

public class OrderController : Controller
{
    private readonly IOrderRepository _orderRepository;
    private readonly ShoppingCart _shoppingCart;

    public OrderController(IOrderRepository orderRepository, ShoppingCart shoppingCart)
    {
        _orderRepository = orderRepository;
        _shoppingCart = shoppingCart;
    }

    public IActionResult Checkout()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Checkout(Order order)
    {
        var items = _shoppingCart.GetShoppingCartItems();
        _shoppingCart.ShoppingCartItems = items;

        if (_shoppingCart.ShoppingCartItems.Count == 0)
        {
            ModelState.AddModelError("", "Your cart is empty, add some sweets first");
        }

        if (ModelState.IsValid)
        {
            order.Status = "معلق"; // Pending fulfillment
            order.PaymentStatus = "Pending";

            _orderRepository.CreateOrder(order);

            // Redirect to the Mock Payment Gateway, passing the newly generated Order ID
            return RedirectToAction("Index", "Payment", new { orderId = order.Id });
        }

        return View(order);
    }

    public IActionResult CheckoutComplete()
    {
        ViewBag.CheckoutCompleteMessage = "Thanks for your order!";
        return View();
    }
}
