using GrpcRecipeClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GrpcRecipeClient.Pages.Recipes;

public class CreateModel : PageModel
{
	[TempData]
	public string? ActionResult { get; set; }
	[BindProperty]
	[Required]
	public Recipe Recipe { get; set; } = new();
	public IEnumerable<string> Categories { get; set; } = Enumerable.Empty<string>();
	[BindProperty]
	public IEnumerable<string>? ChosenCategories { get; set; } = Enumerable.Empty<string>();
	[BindProperty]
	public string? Ingredients { get; set; } = string.Empty;
	[BindProperty]
	public string? Instructions { get; set; } = string.Empty;
	private readonly IHttpClientFactory _httpClientFactory;

	public CreateModel(IHttpClientFactory httpClientFactory) =>
			_httpClientFactory = httpClientFactory;
	public async Task<IActionResult> OnGetAsync()
	{
		try
		{
			await PopulateCategoriesAsync();
			return Page();
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong please try again later";
			return RedirectToPage("./Index");
		}
	}

	public async Task<IActionResult> OnPostAsync()
	{
		try
		{
			await PopulateCategoriesAsync();
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong please try again later";
			return RedirectToPage("./Index");
		}

		if (!ModelState.IsValid)
			return Page();

		Recipe.Id = Guid.Empty;
		if (ChosenCategories != null)
			Recipe.Categories = (List<string>)ChosenCategories;
		if (Ingredients != null)
			Recipe.Ingredients = Ingredients.Split(Environment.NewLine).ToList();
		if (Instructions != null)
			Recipe.Instructions = Instructions.Split(Environment.NewLine).ToList();

		var httpClient = _httpClientFactory.CreateClient("RecipeAPI");
		try
		{
			var response = await httpClient.PostAsJsonAsync("recipes", Recipe);
			response.EnsureSuccessStatusCode();
			ActionResult = "Successfully Created";
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong please try again later";
		}
		return RedirectToPage("./Index");
	}
	public async Task PopulateCategoriesAsync()
	{
		var httpClient = _httpClientFactory.CreateClient("RecipeAPI");
		var response = await httpClient.GetFromJsonAsync<IEnumerable<string>>("categories");
		if (response != null)
			Categories = response;
	}
}
