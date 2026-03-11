using FlorariaOnline.Data;
using FlorariaOnline.Models;
using FlorariaOnline.Services;
using FlorariaOnline.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlorariaOnline.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly CartService _cart;
    private readonly UserManager<IdentityUser> _userManager;

    public CheckoutController(ApplicationDbContext db, CartService cart, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _cart = cart;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var cart = _cart.GetCart();
        if (!cart.Any()) return RedirectToAction("Index", "Cart");

        ViewBag.Total = _cart.Total(cart);
        return View(new CheckoutVm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(CheckoutVm vm)
    {
        var cart = _cart.GetCart();
        if (!cart.Any())
            ModelState.AddModelError("", "Coșul este gol.");

        if (!ModelState.IsValid)
        {
            ViewBag.Total = _cart.Total(cart);
            return View("Index", vm);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Forbid();

        // Verificare stoc + scădere stoc în tranzacție
        using var tx = await _db.Database.BeginTransactionAsync();

        // Încarcă tot ce trebuie din DB
        var allFlowers = await _db.Flowers.ToDictionaryAsync(f => f.Id);

        // Pentru buchete standard, trebuie rețetele
        var productIds = cart.Where(c => c.ItemType == "Standard" && c.ProductId.HasValue)
                             .Select(c => c.ProductId!.Value)
                             .Distinct()
                             .ToList();

        var recipes = await _db.BouquetProductItems
            .Where(i => productIds.Contains(i.BouquetProductId))
            .ToListAsync();

        // 1) Calculă consum total pe flori
        var needed = new Dictionary<int, int>(); // FlowerId -> qty

        foreach (var line in cart)
        {
            if (line.ItemType == "Standard")
            {
                var rid = line.ProductId!.Value;
                var items = recipes.Where(r => r.BouquetProductId == rid);
                foreach (var it in items)
                {
                    var qty = it.Quantity * line.Quantity;
                    needed[it.FlowerId] = needed.GetValueOrDefault(it.FlowerId) + qty;
                }
            }
            else // Custom
            {
                if (line.Flowers == null) continue;
                foreach (var f in line.Flowers)
                {
                    var qty = f.Quantity * line.Quantity;
                    needed[f.FlowerId] = needed.GetValueOrDefault(f.FlowerId) + qty;
                }
            }
        }

        // 2) Verificare stoc
        foreach (var (flowerId, qty) in needed)
        {
            if (!allFlowers.TryGetValue(flowerId, out var fl) || fl.Stock < qty)
            {
                await tx.RollbackAsync();
                ModelState.AddModelError("", $"Stoc insuficient pentru floarea ID={flowerId}. Reîncearcă după actualizare coș.");
                ViewBag.Total = _cart.Total(cart);
                return View("Index", vm);
            }
        }

        // 3) Creează comanda
        var order = new Order
        {
            UserId = user.Id,
            DeliveryName = vm.DeliveryName,
            Phone = vm.Phone,
            Address = vm.Address,
            Notes = vm.Notes,
            PaymentMethod = vm.PaymentMethod,
            Status = OrderStatus.New
        };

        foreach (var line in cart)
        {
            var item = new OrderItem
            {
                ItemType = line.ItemType,
                Name = line.Name,
                UnitPrice = line.UnitPrice,
                Quantity = line.Quantity,
                LineTotal = line.UnitPrice * line.Quantity
            };

            if (line.ItemType == "Custom")
            {
                item.CustomBouquet = new CustomBouquet
                {
                    WrapType = line.WrapType ?? "Classic",
                    GreetingCardMessage = line.GreetingCardMessage,
                    AssemblyFee = line.AssemblyFee ?? 30,
                    Items = (line.Flowers ?? new List<CustomFlowerLine>()).Select(f => new CustomBouquetItem
                    {
                        FlowerId = f.FlowerId,
                        Quantity = f.Quantity
                    }).ToList()
                };
            }

            order.Items.Add(item);
        }

        order.Total = order.Items.Sum(i => i.LineTotal);

        _db.Orders.Add(order);

        // 4) Scade stoc
        foreach (var (flowerId, qty) in needed)
        {
            allFlowers[flowerId].Stock -= qty;
        }

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        _cart.Clear();
        return RedirectToAction("Success", new { id = order.Id });
    }

    public async Task<IActionResult> Success(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Forbid();

        var order = await _db.Orders
            .Include(o => o.Items).ThenInclude(i => i.CustomBouquet)!.ThenInclude(cb => cb.Items)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

        if (order == null) return NotFound();
        return View(order);
    }
}