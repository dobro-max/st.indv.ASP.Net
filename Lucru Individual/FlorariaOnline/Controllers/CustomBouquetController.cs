using FlorariaOnline.Data;
using FlorariaOnline.Services;
using FlorariaOnline.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlorariaOnline.Controllers;

public class CustomBouquetController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly CartService _cart;

    public CustomBouquetController(ApplicationDbContext db, CartService cart)
    {
        _db = db;
        _cart = cart;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var flowers = await _db.Flowers.OrderBy(f => f.Name).ToListAsync();

        var vm = new CustomBouquetCreateVm
        {
            AvailableFlowers = flowers.Select(f => new CustomBouquetCreateVm.FlowerRow
            {
                Id = f.Id,
                Name = f.Name,
                Color = f.Color,
                PricePerStem = f.PricePerStem,
                Stock = f.Stock
            }).ToList()
        };

        // initialize quantities with 0
        foreach (var f in flowers)
            vm.Quantities[f.Id] = 0;

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomBouquetCreateVm vm)
    {
        var flowers = await _db.Flowers.OrderBy(f => f.Name).ToListAsync();
        vm.AvailableFlowers = flowers.Select(f => new CustomBouquetCreateVm.FlowerRow
        {
            Id = f.Id,
            Name = f.Name,
            Color = f.Color,
            PricePerStem = f.PricePerStem,
            Stock = f.Stock
        }).ToList();

        var chosen = vm.Quantities.Where(kv => kv.Value > 0).ToList();
        if (!chosen.Any())
            ModelState.AddModelError("", "Selectează cel puțin o floare pentru buchetul personalizat.");

        if (!ModelState.IsValid)
            return View(vm);

        // build custom bouquet lines + price
        var customLines = new List<CustomFlowerLine>();
        decimal flowersSum = 0;

        foreach (var (flowerId, qty) in chosen)
        {
            var fl = flowers.FirstOrDefault(x => x.Id == flowerId);
            if (fl == null) continue;

            if (qty > fl.Stock)
            {
                ModelState.AddModelError("", $"Stoc insuficient pentru {fl.Name}. Disponibil: {fl.Stock}");
                return View(vm);
            }

            customLines.Add(new CustomFlowerLine
            {
                FlowerId = fl.Id,
                Name = fl.Name,
                PricePerStem = fl.PricePerStem,
                Quantity = qty
            });

            flowersSum += fl.PricePerStem * qty;
        }

        // wrap price (simplu)
        var wrapPrice = vm.WrapType switch
        {
            "Premium" => 25m,
            "Luxury" => 40m,
            _ => 15m
        };

        var unitPrice = flowersSum + vm.AssemblyFee + wrapPrice;

        var key = $"CUS-{Guid.NewGuid():N}";
        _cart.AddOrIncrease(new CartLine
        {
            Key = key,
            ItemType = "Custom",
            Name = $"Buchet personalizat ({vm.WrapType})",
            UnitPrice = unitPrice,
            Quantity = 1,
            WrapType = vm.WrapType,
            GreetingCardMessage = vm.GreetingCardMessage,
            AssemblyFee = vm.AssemblyFee,
            Flowers = customLines
        });

        return RedirectToAction("Index", "Cart");
    }
}