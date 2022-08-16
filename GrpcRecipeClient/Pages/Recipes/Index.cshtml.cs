using GrpcRecipeClient.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GrpcRecipeClient.Pages.Recipes;

public class IndexModel : PageModel
{
	[TempData]
	public string? ActionResult { get; set; }
	private readonly RecipeService.RecipeServiceClient _recipeServiceClient;
	public List<Recipe> RecipeList { get; set; } = new();

	public IndexModel(RecipeService.RecipeServiceClient recipeServiceClient) { 
		_recipeServiceClient = recipeServiceClient;
	}

	public async Task<IActionResult> OnGetAsync()
	{
		try
		{
			var response = await _recipeServiceClient.ListRecipesAsync(new());
			List<Recipe> recipes = response.Recipes.ToList();
			if (recipes != null)
				RecipeList = recipes;
			return Page();
		}
		catch (RpcException ex)
		{
			ActionResult = ex.Status.Detail;
			return RedirectToPage("./Index");
		}
		catch (Exception)
		{
			ActionResult = "Something went wrong please try again later";
			return RedirectToPage("./Index");
		}
	}
}
