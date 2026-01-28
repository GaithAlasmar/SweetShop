using Microsoft.AspNetCore.Mvc;
using SweetShop.Models.Interfaces;
using SweetShop.Models;
using SweetShop.ViewModels;

namespace SweetShop.Controllers;

public class ShoppingCartController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ShoppingCart _shoppingCart;

    public ShoppingCartController(IProductRepository productRepository, ShoppingCart shoppingCart)
    {
        _productRepository = productRepository;
        _shoppingCart = shoppingCart;
    }

    public IActionResult Index()
    {
        var items = _shoppingCart.GetShoppingCartItems();
        _shoppingCart.ShoppingCartItems = items;

        var shoppingCartViewModel = new ShoppingCartViewModel
        {
            ShoppingCart = _shoppingCart,
            ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()
        };

        return View(shoppingCartViewModel);
    }

    public RedirectToActionResult AddToShoppingCart(int productId)
    {
        var selectedProduct = _productRepository.GetAllProducts().FirstOrDefault(p => p.Id == productId);

        if (selectedProduct != null)
        {
            _shoppingCart.AddToCart(selectedProduct, 1);
        }
        return RedirectToAction("Index");
    }

    public RedirectToActionResult RemoveFromShoppingCart(int productId)
    {
        var selectedProduct = _productRepository.GetAllProducts().FirstOrDefault(p => p.Id == productId);

        if (selectedProduct != null)
        {
            _shoppingCart.RemoveFromCart(selectedProduct);
        }
        return RedirectToAction("Index");
    }
}
