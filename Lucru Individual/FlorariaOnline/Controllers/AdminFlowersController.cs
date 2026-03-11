using FlorariaOnline.Data;
using FlorariaOnline.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlorariaOnline.Controllers;

[Authorize(Roles = "Admin")]
public class AdminFlowersController : Controller
{
    private readonly ApplicationDbContext _db;
    public AdminFlowersController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
        => View(await _db.Flowers.OrderBy(f => f.Name).ToListAsync());

    [HttpGet]
    public IActionResult Create() => View(new Flower());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Flower model)
    {
        if (!ModelState.IsValid) return View(model);
        _db.Flowers.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var f = await _db.Flowers.FindAsync(id);
        if (f == null) return NotFound();
        return View(f);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Flower model)
    {
        if (!ModelState.IsValid) return View(model);
        _db.Flowers.Update(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}