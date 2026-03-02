using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecipeBook.Data;
using RecipeBook.Models;

namespace RecipeBook.Controllers;

public class RecipesController(AppDbContext db) : Controller
{
    private readonly AppDbContext _db = db;

    // ── INDEX ─────────────────────────────────────────────────────────────────
    public async Task<IActionResult> Index(string? search, int? categoryId)
    {
        var query = _db.Recipes
            .Include(r => r.Category)
            .Include(r => r.Ingredients)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(r => r.Title.Contains(search) || (r.Description != null && r.Description.Contains(search)));

        if (categoryId.HasValue)
            query = query.Where(r => r.CategoryId == categoryId.Value);

        ViewBag.Categories = new SelectList(await _db.Categories.ToListAsync(), "Id", "Name", categoryId);
        ViewBag.Search = search;
        ViewBag.SelectedCategory = categoryId;
        ViewBag.TotalCount = await _db.Recipes.CountAsync();

        return View(await query.OrderByDescending(r => r.CreatedAt).ToListAsync());
    }

    // ── DETAILS ───────────────────────────────────────────────────────────────
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var recipe = await _db.Recipes
            .Include(r => r.Category)
            .Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.Id == id);

        return recipe == null ? NotFound() : View(recipe);
    }

    // ── CREATE GET ────────────────────────────────────────────────────────────
    public async Task<IActionResult> Create()
    {
        await PopulateCategoriesAsync();
        return View(new Recipe());
    }

    // ── CREATE POST ───────────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Recipe recipe,
        List<string> IngredientNames, List<string> IngredientQuantities)
    {
        // Remove nav-prop validation noise
        ModelState.Remove("Category");
        ModelState.Remove("Ingredients");

        if (ModelState.IsValid)
        {
            recipe.CreatedAt = DateTime.UtcNow;
            recipe.UpdatedAt = DateTime.UtcNow;

            // Attach ingredients from parallel lists
            recipe.Ingredients = IngredientNames
                .Select((name, i) => new Ingredient
                {
                    Name = name.Trim(),
                    Quantity = i < IngredientQuantities.Count ? IngredientQuantities[i].Trim() : null
                })
                .Where(ing => !string.IsNullOrWhiteSpace(ing.Name))
                .ToList();

            _db.Recipes.Add(recipe);
            await _db.SaveChangesAsync();
            TempData["Flash"] = $"\"{recipe.Title}\" added to your recipe book!";
            return RedirectToAction(nameof(Index));
        }

        await PopulateCategoriesAsync(recipe.CategoryId);
        return View(recipe);
    }

    // ── EDIT GET ──────────────────────────────────────────────────────────────
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var recipe = await _db.Recipes
            .Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null) return NotFound();

        await PopulateCategoriesAsync(recipe.CategoryId);
        return View(recipe);
    }

    // ── EDIT POST ─────────────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Recipe recipe,
        List<string> IngredientNames, List<string> IngredientQuantities)
    {
        if (id != recipe.Id) return NotFound();

        ModelState.Remove("Category");
        ModelState.Remove("Ingredients");

        if (ModelState.IsValid)
        {
            // Replace ingredients
            var existing = _db.Ingredients.Where(i => i.RecipeId == id);
            _db.Ingredients.RemoveRange(existing);

            recipe.UpdatedAt = DateTime.UtcNow;
            recipe.Ingredients = IngredientNames
                .Select((name, i) => new Ingredient
                {
                    Name = name.Trim(),
                    Quantity = i < IngredientQuantities.Count ? IngredientQuantities[i].Trim() : null,
                    RecipeId = id
                })
                .Where(ing => !string.IsNullOrWhiteSpace(ing.Name))
                .ToList();

            try
            {
                _db.Update(recipe);
                await _db.SaveChangesAsync();
                TempData["Flash"] = $"\"{recipe.Title}\" updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_db.Recipes.Any(r => r.Id == id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        await PopulateCategoriesAsync(recipe.CategoryId);
        return View(recipe);
    }

    // ── DELETE GET ────────────────────────────────────────────────────────────
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var recipe = await _db.Recipes
            .Include(r => r.Category)
            .FirstOrDefaultAsync(r => r.Id == id);

        return recipe == null ? NotFound() : View(recipe);
    }

    // ── DELETE POST ───────────────────────────────────────────────────────────
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var recipe = await _db.Recipes.FindAsync(id);
        if (recipe != null)
        {
            _db.Recipes.Remove(recipe);
            await _db.SaveChangesAsync();
            TempData["Flash"] = $"\"{ recipe.Title}\" has been deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    // ── HELPER ────────────────────────────────────────────────────────────────
    private async Task PopulateCategoriesAsync(int? selectedId = null)
    {
        ViewBag.Categories = new SelectList(
            await _db.Categories.OrderBy(c => c.Name).ToListAsync(),
            "Id", "Name", selectedId);
    }
}