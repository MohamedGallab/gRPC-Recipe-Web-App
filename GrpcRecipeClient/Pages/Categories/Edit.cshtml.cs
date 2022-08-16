using Grpc.Core;
using GrpcRecipeClient.Protos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GrpcRecipeClient.Pages.Categories;

public class EditModel : PageModel
{
	[TempData]
	public string? ActionResult { get; set; }
	[FromRoute(Name = "category")]
	[Display(Name = "Old Category Name")]
	public string OldCategory { get; set; } = string.Empty;
	[BindProperty]
	[Required]
	[Display(Name = "New Category Name")]
	public string NewCategory { get; set; } = string.Empty;
	private readonly CategoryService.CategoryServiceClient _categoryServiceClient;

	public EditModel(CategoryService.CategoryServiceClient categoryServiceClient) =>
			_categoryServiceClient = categoryServiceClient;

	public void OnGet()
	{
	}
	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
			return Page();
		try
		{
			var response = await _categoryServiceClient.UpdateCategoryAsync(new()
			{
				Title = NewCategory,
				OldTitle = OldCategory
			});
			ActionResult = "Created successfully";
		}
		catch (RpcException ex)
		{
			ActionResult = ex.Status.Detail;
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong, Try again later";
		}
		return RedirectToPage("./Index");
	}
}
