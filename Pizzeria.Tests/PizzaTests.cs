using Microsoft.AspNetCore.Mvc;
using Pizzeria.Api.Models;
using Pizzeria.Controllers;
using Pizzeria.Repositories;
using Xunit;

namespace Pizzeria.Tests;

public sealed class PizzaTests : IDisposable
{
    private readonly string _temporaryDirectory;

    public PizzaTests()
    {
        _temporaryDirectory = Path.Combine(Path.GetTempPath(), $"pizzeria-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_temporaryDirectory);
    }

	[Fact]
	public void Price_is_null_when_an_ingredient_has_no_price()
	{
		var pizza = new PizzaModel
		{
			Ingredients =
			[
				new IngredientModel { Id = "potatoes", Price = null },
				new IngredientModel { Id = "mozzarella", Price = 2m }
			]
		};

		Assert.Null(pizza.Price);
	}

	[Fact]
	public void Price_is_the_sum_of_ingredient_prices()
	{
		var pizza = new PizzaModel
		{
			Ingredients =
			[
				new IngredientModel { Id = "tomato-sauce", Price = 1m },
				new IngredientModel { Id = "mozzarella", Price = 2m },
				new IngredientModel { Id = "mushrooms", Price = 2m }
			]
		};

		Assert.Equal(5m, pizza.Price);
	}

	[Fact]
    public async Task GetById_returns_not_found_for_unknown_pizza()
    {
        var controller = CreateController("[]", "[]");

        var result = await controller.GetById("unknown");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_generates_an_id_and_returns_created_result()
    {
        var controller = CreateController("[]", "[]");
        var pizza = new PizzaModel { Name = "Margherita" };

        var result = await controller.Create(pizza);

        var response = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdPizza = Assert.IsType<PizzaModel>(response.Value);
        Assert.False(string.IsNullOrWhiteSpace(createdPizza.Id));
        Assert.Equal("Margherita", createdPizza.Name);
        Assert.Equal(createdPizza.Id, response.RouteValues!["id"]);
    }

    [Fact]
    public async Task Update_returns_not_found_for_unknown_pizza()
    {
        var controller = CreateController("[]", "[]");

        var result = await controller.Update("unknown", new PizzaModel { Name = "Updated" });

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_removes_an_existing_pizza()
    {
        var controller = CreateController(
            "[{\"id\":\"pizza-1\",\"name\":\"Original\",\"ingredientIds\":[]}]",
            "[]");

        var result = await controller.Delete("pizza-1");
        var remaining = await controller.GetAll();

        Assert.IsType<NoContentResult>(result);
        var response = Assert.IsType<OkObjectResult>(remaining.Result);
        Assert.Empty(Assert.IsAssignableFrom<IEnumerable<PizzaModel>>(response.Value));
    }

    private PizzaController CreateController(string pizzasJson, string ingredientsJson)
    {
        var pizzaPath = Path.Combine(_temporaryDirectory, $"pizza-{Guid.NewGuid():N}.json");
        var ingredientPath = Path.Combine(_temporaryDirectory, $"ingredient-{Guid.NewGuid():N}.json");
        File.WriteAllText(pizzaPath, pizzasJson);
        File.WriteAllText(ingredientPath, ingredientsJson);

        return new PizzaController(
            new PizzaRepository(new IngredientRepository(ingredientPath), pizzaPath));
    }

    public void Dispose()
    {
        if (Directory.Exists(_temporaryDirectory))
            Directory.Delete(_temporaryDirectory, recursive: true);
    }
}