using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ravenapi.DAL.Data;
using ravenapi.DAL.Entities;
using ravenapi.DAL.ViewModels.Category;  // ← added

namespace ravenapi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : BaseController
{
    private readonly ApplicationDbContext _db;

    public CategoriesController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _db.Categories
            .Where(c => c.UserId == CurrentUserId)
            .OrderBy(c => c.Name)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Type,
                c.Icon
            })
            .ToListAsync();

        return Ok(categories);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryViewModel model)  // ← updated
    {
        var category = new Category
        {
            UserId = CurrentUserId,
            Name = model.Name,   // ← req → model
            Type = model.Type,
            Icon = model.Icon
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = category.Id }, new
        {
            category.Id,
            category.Name,
            category.Type,
            category.Icon
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateCategoryViewModel model)  // ← updated
    {
        var category = await _db.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == CurrentUserId);

        if (category is null) return NotFound();

        category.Name = model.Name;   // ← req → model
        category.Type = model.Type;
        category.Icon = model.Icon;

        await _db.SaveChangesAsync();
        return Ok(new { category.Id, category.Name, category.Type, category.Icon });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _db.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == CurrentUserId);

        if (category is null) return NotFound();

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}