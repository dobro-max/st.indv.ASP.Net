using FlorariaOnline.Data;
using FlorariaOnline.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlorariaOnline.Controllers;

[Authorize(Roles = "Admin")]
public class AdminOrdersController : Controller
{
    private readonly ApplicationDbContext _db;
    public AdminOrdersController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var orders = await _db.Orders
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _db.Orders
            .Include(o => o.Items).ThenInclude(i => i.CustomBouquet)!.ThenInclude(cb => cb.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();
        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetStatus(int id, OrderStatus status)
    {
        var o = await _db.Orders.FindAsync(id);
        if (o == null) return NotFound();

        o.Status = status;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id });
    }
}