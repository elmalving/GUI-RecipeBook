using RecipeBook.Models;

namespace RecipeBook.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        if (context.Categories.Any()) return;

        var categories = new List<Category>
        {
            new() { Name = "Breakfast" },
            new() { Name = "Lunch" },
            new() { Name = "Dinner" },
            new() { Name = "Dessert" },
            new() { Name = "Snacks" }
        };
        context.Categories.AddRange(categories);
        context.SaveChanges();

        var recipes = new List<Recipe>
        {
            new()
            {
                Title = "Classic Pancakes",
                Description = "Fluffy, golden pancakes perfect for lazy mornings.",
                Instructions = "1. Mix flour, sugar, baking powder and salt.\n2. Whisk in milk, egg and melted butter.\n3. Cook on medium heat until bubbles form, then flip.\n4. Serve with maple syrup.",
                PrepTimeMinutes = 10,
                CookTimeMinutes = 15,
                Servings = 4,
                CategoryId = categories[0].Id,
                Ingredients = new List<Ingredient>
                {
                    new() { Name = "All-purpose flour", Quantity = "1½ cups" },
                    new() { Name = "Milk", Quantity = "1¼ cups" },
                    new() { Name = "Egg", Quantity = "1 large" },
                    new() { Name = "Butter, melted", Quantity = "3 tbsp" },
                    new() { Name = "Baking powder", Quantity = "1 tsp" },
                    new() { Name = "Sugar", Quantity = "2 tbsp" },
                    new() { Name = "Salt", Quantity = "½ tsp" }
                }
            },
            new()
            {
                Title = "Spaghetti Carbonara",
                Description = "A Roman classic - creamy, rich and ready in 30 minutes.",
                Instructions = "1. Cook spaghetti in salted water until al dente.\n2. Fry pancetta until crisp.\n3. Whisk eggs, parmesan, and black pepper.\n4. Toss hot pasta with pancetta, remove from heat, stir in egg mix.\n5. Serve immediately.",
                PrepTimeMinutes = 5,
                CookTimeMinutes = 20,
                Servings = 2,
                CategoryId = categories[2].Id,
                Ingredients = new List<Ingredient>
                {
                    new() { Name = "Spaghetti", Quantity = "200g" },
                    new() { Name = "Pancetta or guanciale", Quantity = "100g" },
                    new() { Name = "Eggs", Quantity = "2 large" },
                    new() { Name = "Parmesan, grated", Quantity = "50g" },
                    new() { Name = "Black pepper", Quantity = "to taste" }
                }
            },
            new()
            {
                Title = "Chocolate Lava Cake",
                Description = "Warm, gooey and indulgent - the ultimate dessert.",
                Instructions = "1. Melt chocolate and butter together.\n2. Whisk in sugar, eggs and flour.\n3. Pour into buttered ramekins.\n4. Bake at 220°C for 12 minutes.\n5. Invert onto plate and serve warm.",
                PrepTimeMinutes = 15,
                CookTimeMinutes = 12,
                Servings = 4,
                CategoryId = categories[3].Id,
                Ingredients = new List<Ingredient>
                {
                    new() { Name = "Dark chocolate", Quantity = "100g" },
                    new() { Name = "Butter", Quantity = "100g" },
                    new() { Name = "Eggs", Quantity = "2" },
                    new() { Name = "Egg yolks", Quantity = "2" },
                    new() { Name = "Caster sugar", Quantity = "3 tbsp" },
                    new() { Name = "Plain flour", Quantity = "2 tbsp" }
                }
            }
        };

        context.Recipes.AddRange(recipes);
        context.SaveChanges();
    }
}