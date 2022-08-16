using Grpc.Core;
using GrpcRecipeBackend.Protos;
using Google.Protobuf;

namespace GrpcRecipeBackend.Services;

public class RecipeService : Protos.RecipeService.RecipeServiceBase
{
	private static List<Recipe> s_recipesList = new();

	public void LoadData()
	{
		// load previous recipes if exists
		string recipesFile = "Recipes.json";

		if (File.Exists(recipesFile))
		{
			using var input = File.OpenRead(recipesFile);
			s_recipesList = ListRecipesResponse.Parser.ParseFrom(input).Recipes.ToList();
		}
		else
		{
			File.Create(recipesFile).Dispose();
		}
	}

	public void SaveData()
	{
		var temp = new ListRecipesResponse();
		temp.Recipes.AddRange(s_recipesList.OrderBy(o => o.Title).ToList());
		using var output = File.Create("Recipes.json");
		temp.WriteTo(output);
	}

	public override Task<ListRecipesResponse> ListRecipes(Google.Protobuf.WellKnownTypes.Empty request, ServerCallContext context)
	{
		LoadData();
		ListRecipesResponse response = new();
		response.Recipes.AddRange(s_recipesList);
		return Task.FromResult(response);
	}

	public override Task<Recipe> CreateRecipe(Recipe recipe, ServerCallContext context)
	{
		LoadData();

		if(recipe.Title == null)
		{
			const string msg = "Invalid Recipe";
			throw new RpcException(new Status(StatusCode.InvalidArgument, msg));
		}

		recipe.Id = Guid.NewGuid().ToString();
		s_recipesList.Add(recipe);
		SaveData();
		return Task.FromResult(recipe);
	}

	public override Task<Recipe> ReadRecipe(Recipe recipe, ServerCallContext context)
	{
		LoadData();

		if (s_recipesList.Find(r => r.Id == recipe.Id) is Recipe foundRecipe)
		{
			return Task.FromResult(foundRecipe);
		}
		const string msg = "Could not find recipe";
		throw new RpcException(new Status(StatusCode.NotFound, msg));
	}

	public override Task<Recipe> UpdateRecipe(Recipe recipe, ServerCallContext context)
	{
		LoadData();

		if (s_recipesList.Find(r => r.Id == recipe.Id) is Recipe oldRecipe)
		{
			s_recipesList.Remove(oldRecipe);
			s_recipesList.Add(recipe);
			SaveData();
			return Task.FromResult(recipe);
		}

		const string msg = "Could not find recipe";
		throw new RpcException(new Status(StatusCode.NotFound, msg));
	}

	public override Task<Recipe> DeleteRecipe(Recipe recipe, ServerCallContext context)
	{
		LoadData();

		if (s_recipesList.Find(r => r.Id == recipe.Id) is Recipe oldRecipe)
		{
			s_recipesList.Remove(oldRecipe);
			SaveData();
			return Task.FromResult(recipe);
		}

		const string msg = "Could not find recipe";
		throw new RpcException(new Status(StatusCode.NotFound, msg));
	}
}