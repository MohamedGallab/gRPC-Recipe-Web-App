using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GrpcRecipeClient.Pages.Categories;

public class IndexModel : PageModel
{
	[TempData]
	public string? ActionResult { get; set; }
	private readonly IHttpClientFactory _httpClientFactory;

	public IndexModel(IHttpClientFactory httpClientFactory) =>
		_httpClientFactory = httpClientFactory;

	public List<string> CategoryList { get; set; } = new();

	public async Task<IActionResult> OnGetAsync()
	{
		try
		{
			var httpClient = _httpClientFactory.CreateClient("RecipeAPI");
			List<string>? categories = await httpClient.GetFromJsonAsync<List<string>>("categories");
			if (categories != null)
				CategoryList = categories;
			return Page();
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong, Try again later";
			return RedirectToPage("/Index");
		}
	}
}
