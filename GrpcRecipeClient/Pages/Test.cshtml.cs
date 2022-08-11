using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcRecipeClient;
using GrpcRecipeClient.Protos;

namespace GrpcRecipeClient.Pages;

public class TestModel : PageModel
{

	private readonly RecipeService.RecipeServiceClient _client;

	public TestModel(RecipeService.RecipeServiceClient client) =>
		_client = client;

	public async Task OnGetAsync()
	{
		await UpdateRecipe();
	}

	public async Task CreateRecipe()
	{
		var reply = await _client.CreateRecipeAsync(new Recipe
		{
			Title = "tuna salad",
			Categories = { "lunch", "dinner" },
			Ingredients = { "tuna can" },
			Instructions = { "open", "eat" }
		});
		Console.WriteLine(reply);
	}

	public async Task ListRecipes()
	{
		var reply = await _client.ListRecipesAsync(new Google.Protobuf.WellKnownTypes.Empty());
		foreach (var recipe in reply.Recipes)
		{
			Console.WriteLine("Greeting: " + recipe);
		}
		Console.WriteLine("Press any key to exit...");
	}

	public async Task ReadRecipe()
	{
		var reply = await _client.ReadRecipeAsync(new Recipe
		{
			Id = "1"
		});
		Console.WriteLine(reply);
	}

	public async Task UpdateRecipe()
	{
		var reply = await _client.UpdateRecipeAsync(new Recipe
		{
			Id = "1",
			Title = "Fresh Tuna Salad",
			Categories = { "lunch", "snack" },
			Ingredients = { "tuna can" },
			Instructions = { "open", "eat" }
		});
		Console.WriteLine(reply);
	}

	public async Task DeleteRecipe()
	{
		var reply = await _client.DeleteRecipeAsync(new Recipe
		{
			Id = "1"
		});
		Console.WriteLine(reply);
	}
}
