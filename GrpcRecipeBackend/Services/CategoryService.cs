using Grpc.Core;
using Google.Protobuf;
using GrpcRecipeBackend.Protos;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;

namespace GrpcRecipeBackend.Services;

public class CategoryService : Protos.CategoryService.CategoryServiceBase
{
	private static List<string> s_categoriesList = new();
	private static List<Recipe> s_recipesList = new();
	private readonly string _recipesFile = "Recipes.json";
	private readonly string _categoriesFile = "Categories.json";

	public async Task LoadDataAsync()
	{
		// load previous recipes if exists
		if (File.Exists(_recipesFile))
		{
			var jsonRecipesString = await File.ReadAllTextAsync(_recipesFile);
			s_recipesList = JsonConvert.DeserializeObject<List<Recipe>>(jsonRecipesString)!;
		}
		else
		{
			File.Create(_recipesFile).Dispose();
		}

		// load previous categories if exists
		if (File.Exists(_categoriesFile))
		{
			var jsonCategoriesString = await File.ReadAllTextAsync(_categoriesFile);
			s_categoriesList = JsonConvert.DeserializeObject<List<string>>(jsonCategoriesString)!;
		}
		else
		{
			File.Create(_categoriesFile).Dispose();
		}
	}

	public async Task SaveDataAsync()
	{
		await Task.WhenAll(
			File.WriteAllTextAsync(_recipesFile, JsonConvert.SerializeObject(
				s_recipesList.OrderBy(o => o.Title).ToList(), Formatting.Indented)),

			File.WriteAllTextAsync(_categoriesFile, JsonConvert.SerializeObject(
				s_categoriesList.OrderBy(o => o).ToList(), Formatting.Indented))
		);
	}

	public override async Task<ListCategoriesResponse> ListCategories(Empty request, ServerCallContext context)
	{
		await LoadDataAsync();
		ListCategoriesResponse response = new();
		response.Categories.AddRange(s_categoriesList);
		return response;
	}

	public override async Task<Category> CreateCategory(Category request, ServerCallContext context)
	{
		await LoadDataAsync();

		if (request.Title == null)
		{
			const string msg = "Invalid Category";
			throw new RpcException(new Status(StatusCode.InvalidArgument, msg));
		}

		if (s_categoriesList.Find(r => r == request.Title) is string category)
		{
			const string msg = "Category already exists";
			throw new RpcException(new Status(StatusCode.AlreadyExists, msg));
		}

		s_categoriesList.Add(request.Title);
		await SaveDataAsync();
		return request;
	}

	public override async Task<Category> UpdateCategory(Category request, ServerCallContext context)
	{
		await LoadDataAsync();

		if (s_categoriesList.Find(r => r == request.OldTitle) is string oldCategory)
		{
			s_categoriesList.Remove(oldCategory);
			s_categoriesList.Add(request.Title);
			foreach (var recipe in s_recipesList)
			{
				if (recipe.Categories.Contains(oldCategory))
				{
					recipe.Categories.Remove(oldCategory);
					recipe.Categories.Add(request.Title);
				}
			}
			await SaveDataAsync();
			return request;
		}

		const string msg = "Could not find category";
		throw new RpcException(new Status(StatusCode.NotFound, msg));
	}

	public override async Task<Category> DeleteCategory(Category request, ServerCallContext context)
	{
		await LoadDataAsync();

		if (s_categoriesList.Find(r => r == request.Title) is string oldCategory)
		{
			s_categoriesList.Remove(oldCategory);
			foreach (Recipe recipe in s_recipesList)
			{
				recipe.Categories.Remove(oldCategory);
			}
			await SaveDataAsync();
			return request;
		}

		const string msg = "Could not find category";
		throw new RpcException(new Status(StatusCode.NotFound, msg));
	}
}
