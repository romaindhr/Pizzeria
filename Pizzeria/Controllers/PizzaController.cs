using Microsoft.AspNetCore.Mvc;
using Pizzeria.Api.Models;
using Pizzeria.Repositories;

namespace Pizzeria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PizzaController : ControllerBase
{
	private readonly PizzaRepository _repository;

	public PizzaController(PizzaRepository repository)
	{
		_repository = repository;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<PizzaModel>>> GetAll()
	{
		return Ok(await _repository.GetAllAsync());
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<PizzaModel>> GetById(string id)
	{
		var pizza = await _repository.GetByIdAsync(id);
		return pizza is null ? NotFound() : Ok(pizza);
	}

	[HttpPost]
	public async Task<ActionResult<PizzaModel>> Create(PizzaModel pizza)
	{
		var pizzaToAdd = new PizzaModel
		{
			Id = string.IsNullOrWhiteSpace(pizza.Id) ? Guid.NewGuid().ToString("N") : pizza.Id,
			Name = pizza.Name,
			Ingredients = pizza.Ingredients
		};

		await _repository.AddAsync(pizzaToAdd);

		return CreatedAtAction(nameof(GetById), new { id = pizzaToAdd.Id }, pizzaToAdd);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(string id, PizzaModel pizza)
	{
		var existing = await _repository.GetByIdAsync(id);
		if (existing is null)
			return NotFound();

		await _repository.UpdateAsync(new PizzaModel
		{
			Id = id,
			Name = pizza.Name,
			Ingredients = pizza.Ingredients
		});

		return NoContent();
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(string id)
	{
		var pizza = await _repository.GetByIdAsync(id);
		if (pizza is null)
			return NotFound();

		await _repository.DeleteAsync(id);
		return NoContent();
	}
}
