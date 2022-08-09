using GrpcRecipeClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GrpcRecipeClient.Pages.Recipes;

public class DeleteModel : PageModel
{
	[TempData]
	public string? ActionResult { get; set; }
	[BindProperty(SupportsGet = true)]
	public Guid RecipeId { get; set; } = Guid.Empty;
	public Recipe Recipe { get; set; } = new();
	private readonly IHttpClientFactory _httpClientFactory;

	public DeleteModel(IHttpClientFactory httpClientFactory) =>
			_httpClientFactory = httpClientFactory;

	public async Task<IActionResult> OnGet()
	{
		try
		{
			var httpClient = _httpClientFactory.CreateClient("RecipeAPI");
			var response = await httpClient.GetFromJsonAsync<Recipe>($"recipes/{RecipeId}");
			if (response == null)
				return NotFound();
			Recipe = response;
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
			var httpClient = _httpClientFactory.CreateClient("RecipeAPI");
			var response = await httpClient.DeleteAsync($"recipes/{RecipeId}");
			response.EnsureSuccessStatusCode();
			ActionResult = "Successfully Deleted";
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong please try again later";
		}
		return RedirectToPage("./Index");
	}
}

