using GrpcRecipeClient.Protos;
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
	public string Title { get; set; } = string.Empty;
	public IEnumerable<string> Categories { get; set; } = Enumerable.Empty<string>();
	[BindProperty]
	public IEnumerable<string>? ChosenCategories { get; set; } = Enumerable.Empty<string>();
	[BindProperty]
	public string? Ingredients { get; set; } = string.Empty;
	[BindProperty]
	public string? Instructions { get; set; } = string.Empty;
	private readonly RecipeService.RecipeServiceClient _recipeServiceClient;
	private readonly CategoryService.CategoryServiceClient _categoryServiceClient;
	public CreateModel(RecipeService.RecipeServiceClient recipeServiceClient, CategoryService.CategoryServiceClient categoryServiceClient)
	{
		_recipeServiceClient = recipeServiceClient;
		_categoryServiceClient = categoryServiceClient;
	}
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
		// reload category options in case of validation fail
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

		Recipe recipe = new()
		{
			Title = Title
		};

		recipe.Categories.AddRange(ChosenCategories);
		if (Ingredients != null)
			recipe.Ingredients.AddRange(Ingredients.Split(Environment.NewLine).ToList());
		if (Instructions != null)
			recipe.Instructions.AddRange(Instructions.Split(Environment.NewLine).ToList());

		try
		{
			var reply = await _recipeServiceClient.CreateRecipeAsync(recipe);
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
		var response = await _categoryServiceClient.ListCategoriesAsync(new());
		if (response != null)
			Categories = response.Categories;
	}
}
