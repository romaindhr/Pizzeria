namespace Pizzeria.Api.Models;

public class PizzaModel
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;

    public IngredientModel[] Ingredients { get; init; } = [];

    public decimal? Price =>
        Ingredients.Any(i => i.Price is null)
            ? null
            : Ingredients.Sum(i => i.Price!.Value);
}

public sealed class PizzaData
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string[] IngredientIds { get; set; } = [];
}
