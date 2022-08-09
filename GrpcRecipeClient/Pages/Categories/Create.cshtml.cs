using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GrpcRecipeClient.Pages.Categories;

public class CreateModel : PageModel
{
	[TempData]
	public string? ActionResult { get; set; }
	[BindProperty]
	[Required]
	[Display(Name = "Category Name")]
	public string Category { get; set; } = string.Empty;
	private readonly IHttpClientFactory _httpClientFactory;

	public CreateModel(IHttpClientFactory httpClientFactory) =>
			_httpClientFactory = httpClientFactory;
	public void OnGet()
	{
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
			return Page();
		try
		{
			var httpClient = _httpClientFactory.CreateClient("RecipeAPI");
			var response = await httpClient.PostAsJsonAsync("categories?category=" + Category, Category);
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
