using FlorariaOnline.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlorariaOnline.Controllers;

public class CatalogController : Controller
{
    private readonly ApplicationDbContext _db;
    public CatalogController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? q, decimal? min, decimal? max)
    {
        var products = _db.BouquetProducts.Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(q))
            products = products.Where(p => p.Name.Contains(q) || (p.Description ?? "").Contains(q));

        if (min.HasValue) products = products.Where(p => p.BasePrice >= min.Value);
        if (max.HasValue) products = products.Where(p => p.BasePrice <= max.Value);

        var list = await products
            .OrderBy(p => p.Name)
            .ToListAsync();

        ViewBag.Query = q;
        ViewBag.Min = min;
        ViewBag.Max = max;

        return View(list);
    }
}