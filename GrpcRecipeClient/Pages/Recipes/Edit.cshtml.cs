using GrpcRecipeClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GrpcRecipeClient.Pages.Recipes;

public class EditModel : PageModel
{
	[TempData]
	public string? ActionResult { get; set; }
	[BindProperty]
	[Required]
	public Recipe Recipe { get; set; } = new();
	public IEnumerable<string> Categories { get; set; } = Enumerable.Empty<string>();
	[BindProperty]
	public IEnumerable<string> ChosenCategories { get; set; } = Enumerable.Empty<string>();
	[BindProperty]
	public string Ingredients { get; set; } = string.Empty;
	[BindProperty]
	public string Instructions { get; set; } = string.Empty;
	private readonly IHttpClientFactory _httpClientFactory;

	public EditModel(IHttpClientFactory httpClientFactory) =>
			_httpClientFactory = httpClientFactory;
	public async Task<IActionResult> OnGetAsync(Guid recipeId)
	{
		await PopulateRecipeAndCategoriesAsync(recipeId);
		if (Recipe == null)
			return NotFound();
		Ingredients = string.Join(Environment.NewLine, Recipe.Ingredients);
		Instructions = string.Join(Environment.NewLine, Recipe.Instructions);
		return Page();
	}

	public async Task<IActionResult> OnPostAsync(Guid recipeId)
	{
		if (!ModelState.IsValid)
		{
			await PopulateRecipeAndCategoriesAsync(recipeId);
			return Page();
		}

		Recipe.Id = recipeId;
		if (ChosenCategories != null)
			Recipe.Categories = (List<string>)ChosenCategories;
		if (Ingredients != null)
			Recipe.Ingredients = Ingredients.Split(Environment.NewLine).ToList();
		if (Instructions != null)
			Recipe.Instructions = Instructions.Split(Environment.NewLine).ToList();

		var httpClient = _httpClientFactory.CreateClient("RecipeAPI");
		try
		{
			var response = await httpClient.PutAsJsonAsync($"recipes/{Recipe.Id}", Recipe);
			response.EnsureSuccessStatusCode();
			ActionResult = "Successfully Edited";
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong please try again later";
		}
		return RedirectToPage("./Index");
	}

	public async Task PopulateRecipeAndCategoriesAsync(Guid recipeId)
	{
		var httpClient = _httpClientFactory.CreateClient("RecipeAPI");
		var categoriesResponse = await httpClient.GetFromJsonAsync<IEnumerable<string>>("categories");
		if (categoriesResponse != null)
			Categories = categoriesResponse;
		var recipeResponse = await httpClient.GetFromJsonAsync<Recipe>($"recipes/{recipeId}");
		if (recipeResponse != null)
			Recipe = recipeResponse;
	}
}
