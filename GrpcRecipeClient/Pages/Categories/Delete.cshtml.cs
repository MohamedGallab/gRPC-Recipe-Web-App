using GrpcRecipeClient.Protos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GrpcRecipeClient.Pages.Categories;

public class DeleteModel : PageModel
{
	[TempData]
	public string? ActionResult { get; set; }
	[FromRoute(Name = "category")]
	public string Category { get; set; } = string.Empty;
	private readonly CategoryService.CategoryServiceClient _categoryServiceClient;

	public DeleteModel(CategoryService.CategoryServiceClient categoryServiceClient) =>
			_categoryServiceClient = categoryServiceClient;

	public void OnGet()
	{
	}

	public async Task<IActionResult> OnPostAsync()
	{
		try
		{
			var response = await _categoryServiceClient.DeleteCategoryAsync(new() { Title = Category });
			ActionResult = "Created successfully";
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong, Try again later";
		}
		return RedirectToPage("./Index");
	}
}
