﻿using Grpc.Core;
using Google.Protobuf;
using GrpcRecipeBackend.Protos;
using Google.Protobuf.WellKnownTypes;

namespace GrpcRecipeBackend.Services;

public class CategoryService : Protos.CategoryService.CategoryServiceBase
{
	private static List<string> s_categoriesList = new();
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

		// load previous categories if exists
		string categoriesFile = "Categories.json";

		if (File.Exists(categoriesFile))
		{
			using var input = File.OpenRead(categoriesFile);
			s_categoriesList = ListCategoriesResponse.Parser.ParseFrom(input).Categories.ToList();
		}
		else
		{
			File.Create(categoriesFile).Dispose();
		}
	}

	public void SaveData()
	{
		var tempRecipes = new ListRecipesResponse();
		tempRecipes.Recipes.AddRange(s_recipesList.OrderBy(o => o.Title).ToList());
		using var outputRecipes = File.Create("Recipes.json");
		tempRecipes.WriteTo(outputRecipes);

		var tempCategories = new ListCategoriesResponse();
		tempCategories.Categories.AddRange(s_categoriesList.OrderBy(o => o).ToList());
		using var outputCategories = File.Create("Categories.json");
		tempCategories.WriteTo(outputCategories);
	}

	public override Task<ListCategoriesResponse> ListCategories(Empty request, ServerCallContext context)
	{
		LoadData();
		ListCategoriesResponse response = new();
		response.Categories.AddRange(s_categoriesList);
		return Task.FromResult(response);
	}

	public override Task<Category> CreateCategory(Category request, ServerCallContext context)
	{
		LoadData();

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
		SaveData();
		return Task.FromResult(request);
	}

	public override Task<Category> UpdateCategory(Category request, ServerCallContext context)
	{
		LoadData();

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
			SaveData();
			return Task.FromResult(request);
		}

		const string msg = "Could not find category";
		throw new RpcException(new Status(StatusCode.NotFound, msg));
	}

	public override Task<Category> DeleteCategory(Category request, ServerCallContext context)
	{
		LoadData();

		if (s_categoriesList.Find(r => r == request.Title) is string oldCategory)
		{
			s_categoriesList.Remove(oldCategory);
			foreach (Recipe recipe in s_recipesList)
			{
				recipe.Categories.Remove(oldCategory);
			}
			SaveData();
			return Task.FromResult(request);
		}

		const string msg = "Could not find category";
		throw new RpcException(new Status(StatusCode.NotFound, msg));
	}
}
