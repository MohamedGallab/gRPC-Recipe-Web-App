using Grpc.Core;
using GrpcRecipeBackend.Protos;
using System.Text.Json;

namespace GrpcRecipeBackend.Services;

public class RecipeService : Protos.RecipeService.RecipeServiceBase
{

	private static List<Recipe> s_recipesList = new();
	private List<string> _categoriesList = new();

	public RecipeService()
	{
		LoadRecipes();
	}

	async public Task LoadRecipes()
	{
		// load previous recipes if exists
		string recipesFile = "Recipes.json";
		string jsonRecipesString;

		if (File.Exists(recipesFile))
		{
			if (new FileInfo(recipesFile).Length > 0)
			{
				jsonRecipesString = await File.ReadAllTextAsync(recipesFile);
				s_recipesList = JsonSerializer.Deserialize<List<Recipe>>(jsonRecipesString)!;
			}
		}
		else
		{
			File.Create(recipesFile).Dispose();
		}
	}

	public override Task<ListRecipesResponse> ListRecipes(Google.Protobuf.WellKnownTypes.Empty request, ServerCallContext context)
	{
		var response = new ListRecipesResponse();

		foreach (var recipe in s_recipesList)
		{
			response.Recipes.Add(recipe);
		}

		return Task.FromResult(response);
	}
}