using System.Text.Json;
using IngredientModel = Pizzeria.Api.Models.IngredientModel;

namespace Pizzeria.Repositories;

public class IngredientRepository
{
	private readonly string _filePath;
	private readonly JsonSerializerOptions _jsonOptions = new()
	{
		PropertyNameCaseInsensitive = true,
		WriteIndented = true
	};

	public IngredientRepository(string? filePath = null)
	{
		_filePath = filePath
			?? Path.Combine(AppContext.BaseDirectory, "Data", "ingredient.json");
	}

	public IReadOnlyList<IngredientModel> GetAll()
	{
		if (!File.Exists(_filePath))
			return Array.Empty<IngredientModel>();

		var json = File.ReadAllText(_filePath);
		return JsonSerializer.Deserialize<List<IngredientModel>>(json, _jsonOptions)
			   ?? new List<IngredientModel>();
	}

	public IngredientModel? GetById(string id)
	{
		return GetAll().FirstOrDefault(ingredient => ingredient.Id == id);
	}

	public void Add(IngredientModel ingredient)
	{
		var ingredients = GetAll().ToList();

		if (ingredients.Any(item => item.Id == ingredient.Id))
			throw new InvalidOperationException($"L'ingrédient {ingredient.Id} existe déjà.");

		ingredients.Add(ingredient);
		Save(ingredients);
	}

	public bool Update(IngredientModel ingredient)
	{
		var ingredients = GetAll().ToList();
		var index = ingredients.FindIndex(item => item.Id == ingredient.Id);

		if (index < 0)
			return false;

		ingredients[index] = ingredient;
		Save(ingredients);
		return true;
	}

	public bool Delete(string id)
	{
		var ingredients = GetAll().ToList();
		var removed = ingredients.RemoveAll(ingredient => ingredient.Id == id) > 0;

		if (removed)
			Save(ingredients);

		return removed;
	}

	private void Save(IEnumerable<IngredientModel> ingredients)
	{
		var directory = Path.GetDirectoryName(_filePath);
		if (!string.IsNullOrWhiteSpace(directory))
			Directory.CreateDirectory(directory);

		var json = JsonSerializer.Serialize(ingredients, _jsonOptions);
		File.WriteAllText(_filePath, json);
	}
}
