using FlorariaOnline.Data;
using FlorariaOnline.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlorariaOnline.Controllers;

public class CartController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly CartService _cart;

    public CartController(ApplicationDbContext db, CartService cart)
    {
        _db = db;
        _cart = cart;
    }

    public IActionResult Index()
    {
        var cart = _cart.GetCart();
        ViewBag.Total = _cart.Total(cart);
        return View(cart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddStandard(int productId, int quantity = 1)
    {
        if (quantity <= 0) quantity = 1;

        var p = await _db.BouquetProducts.FindAsync(productId);
        if (p == null || !p.IsActive) return NotFound();

        _cart.AddOrIncrease(new CartLine
        {
            Key = $"STD-{p.Id}",
            ItemType = "Standard",
            ProductId = p.Id,
            Name = p.Name,
            UnitPrice = p.BasePrice,
            Quantity = quantity
        });

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetQty(string key, int qty)
    {
        _cart.SetQuantity(key, qty);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(string key)
    {
        _cart.Remove(key);
        return RedirectToAction("Index");
    }
}