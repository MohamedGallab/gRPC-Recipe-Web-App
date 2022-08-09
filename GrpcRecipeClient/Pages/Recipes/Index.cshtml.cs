using GrpcRecipeClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;

namespace GrpcRecipeClient.Pages.Recipes;

public class IndexModel : PageModel
{
	[TempData]
	public string? ActionResult { get; set; }
	private readonly IHttpClientFactory _httpClientFactory;

	public IndexModel(IHttpClientFactory httpClientFactory) =>
		_httpClientFactory = httpClientFactory;

	public List<Recipe> RecipeList { get; set; } = new();

	public async Task<IActionResult> OnGetAsync()
	{
		try
		{
			var httpClient = _httpClientFactory.CreateClient("RecipeAPI");
			List<Recipe>? recipes = await httpClient.GetFromJsonAsync<List<Recipe>>("recipes");
			if (recipes != null)
				RecipeList = recipes;
			return Page();
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong, Try again later";
			return RedirectToPage("/Index");
		}
	}
}
