namespace Pizzeria.Api.Models;

public class IngredientModel
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public decimal? Price { get; init; }

    public string? Category { get; init; }
}
