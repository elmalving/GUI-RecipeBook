using System.ComponentModel.DataAnnotations;

namespace RecipeBook.Models;

public class Ingredient
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Quantity { get; set; }

    // FK
    public int RecipeId { get; set; }
    public Recipe? Recipe { get; set; }
}