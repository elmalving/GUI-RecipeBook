using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeBook.Models;

public class Recipe
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [Display(Name = "Instructions")]
    public string Instructions { get; set; } = string.Empty;

    [Range(1, 600)]
    [Display(Name = "Prep Time (mins)")]
    public int PrepTimeMinutes { get; set; }

    [Range(1, 600)]
    [Display(Name = "Cook Time (mins)")]
    public int CookTimeMinutes { get; set; }

    [Range(1, 100)]
    public int Servings { get; set; } = 4;

    [Display(Name = "Image URL")]
    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // FK
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    // Navigation
    public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

    [NotMapped]
    public int TotalTimeMinutes => PrepTimeMinutes + CookTimeMinutes;
}