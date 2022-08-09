using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GrpcRecipeClient.Pages.Categories;

public class DeleteModel : PageModel
{
	[TempData]
	public string? ActionResult { get; set; }
	[FromRoute(Name = "category")]
	public string Category { get; set; } = string.Empty;
	private readonly IHttpClientFactory _httpClientFactory;

	public DeleteModel(IHttpClientFactory httpClientFactory) =>
			_httpClientFactory = httpClientFactory;

	public void OnGet()
	{
	}

	public async Task<IActionResult> OnPostAsync()
	{
		try
		{
			var httpClient = _httpClientFactory.CreateClient("RecipeAPI");
			var response = await httpClient.DeleteAsync($"categories/{Category}");
			response.EnsureSuccessStatusCode();
			ActionResult = "Created successfully";
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong, Try again later";
		}
		return RedirectToPage("./Index");
	}
}
