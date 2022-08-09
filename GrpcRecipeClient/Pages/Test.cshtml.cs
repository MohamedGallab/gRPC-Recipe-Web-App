using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcRecipeClient;

namespace GrpcRecipeClient.Pages;

public class TestModel : PageModel
{

	private readonly Greeter.GreeterClient _client;

	public TestModel(Greeter.GreeterClient client) =>
		_client = client;

	public async Task OnGetAsync()
	{
		var reply = await _client.SayHelloAsync(
						  new HelloRequest { Name = "GreeterClient" });
		Console.WriteLine("Greeting: " + reply.Message);
		Console.WriteLine("Press any key to exit...");
	}
}
