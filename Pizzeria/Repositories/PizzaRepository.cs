using System.Text.Json;
using PizzaModel = Pizzeria.Api.Models.PizzaModel;
using PizzaData = Pizzeria.Api.Models.PizzaData;

namespace Pizzeria.Repositories;

public class PizzaRepository
{
	private readonly string _filePath;
	private readonly JsonSerializerOptions _jsonOptions = new()
	{
		PropertyNameCaseInsensitive = true,
		WriteIndented = true
	};
	private readonly IngredientRepository _ingredientRepository;

	public PizzaRepository(
		IngredientRepository ingredientRepository,
		string? filePath = null)
	{
		_ingredientRepository = ingredientRepository;

		_filePath = filePath
			?? Path.Combine(AppContext.BaseDirectory, "Data", "pizza.json");
	}

	public async Task<IReadOnlyList<PizzaModel>> GetAllAsync()
	{
		if (!File.Exists(_filePath))
			return Array.Empty<PizzaModel>();

		await using var stream = File.OpenRead(_filePath);

		var pizzas = await JsonSerializer.DeserializeAsync<List<PizzaData>>(
			stream,
			_jsonOptions) ?? [];

		var ingredients = _ingredientRepository
			.GetAll()
			.ToDictionary(ingredient => ingredient.Id);

		return pizzas
			.Select(pizza => new PizzaModel
			{
				Id = pizza.Id,
				Name = pizza.Name,
				Ingredients = pizza.IngredientIds
					.Where(ingredients.ContainsKey)
					.Select(id => ingredients[id])
					.ToArray()
			})
			.ToList();
	}

	public async Task<PizzaModel?> GetByIdAsync(string id)
	{
		var pizzas = await GetAllAsync();
		return pizzas.FirstOrDefault(p => p.Id == id);
	}

	public async Task AddAsync(PizzaModel pizza)
	{
		var pizzas = (await GetAllAsync()).ToList();
		pizzas.Add(pizza);
		await SaveAsync(pizzas);
	}

	public async Task UpdateAsync(PizzaModel pizza)
	{
		var pizzas = (await GetAllAsync()).ToList();
		var index = pizzas.FindIndex(p => p.Id == pizza.Id);

		if (index < 0)
			throw new KeyNotFoundException($"Pizza with id {pizza.Id} was not found.");

		pizzas[index] = pizza;
		await SaveAsync(pizzas);
	}

	public async Task DeleteAsync(string id)
	{
		var pizzas = (await GetAllAsync()).ToList();
		if (pizzas.RemoveAll(p => p.Id == id) > 0)
			await SaveAsync(pizzas);
	}

	private async Task SaveAsync(List<PizzaModel> pizzas)
	{
		var directory = Path.GetDirectoryName(_filePath);
		if (!string.IsNullOrEmpty(directory))
			Directory.CreateDirectory(directory);

		await using var stream = File.Create(_filePath);
		await JsonSerializer.SerializeAsync(stream, pizzas, _jsonOptions);
	}
}
