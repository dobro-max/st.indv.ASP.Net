using FlorariaOnline.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlorariaOnline.Controllers;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _db;
    public ProductController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Details(int id)
    {
        var p = await _db.BouquetProducts
            .Include(x => x.Items).ThenInclude(i => i.Flower)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);

        if (p == null) return NotFound();
        return View(p);
    }
}