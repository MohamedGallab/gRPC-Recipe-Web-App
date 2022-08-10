using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcRecipeClient;
using GrpcRecipeClient.Protos;

namespace GrpcRecipeClient.Pages;

public class TestModel : PageModel
{

	private readonly RecipeService.RecipeServiceClient _client;

	public TestModel(RecipeService.RecipeServiceClient client) =>
		_client = client;

	public async Task OnGetAsync()
	{
		var reply = await _client.ListRecipesAsync(new Google.Protobuf.WellKnownTypes.Empty());
		foreach (var recipe in reply.Recipes)
		{
			Console.WriteLine("Greeting: " + recipe);
		}
		Console.WriteLine("Press any key to exit...");
	}

	//private readonly Greeter.GreeterClient _client;

	//public TestModel(Greeter.GreeterClient client) =>
	//	_client = client;

	//public async Task OnGetAsync()
	//{
	//	var reply = await _client.SayHelloAsync(
	//					  new HelloRequest { Name = "GreeterClient" });
	//	Console.WriteLine("Greeting: " + reply.Message);
	//	Console.WriteLine("Press any key to exit...");
	//}
}
