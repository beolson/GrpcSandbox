using Grpc.Net.Client;
using System.Threading.Tasks;
using ToDoService;



foreach (var second in Enumerable.Range(1, 3))
{
    Console.WriteLine($"starting in {3 - second} seconds");
    await Task.Delay(1000);
}

// The port number must match the port of the gRPC server.
using var channel = GrpcChannel.ForAddress("https://localhost:7117");

var client = new ToDo.ToDoClient(channel);

var reply = await client.CreateToDoAsync(new CreateToDoRequest() { Task = "Do Stuff" });

Console.WriteLine("To Do Created: " + reply.Id);

var response = await client.GetToDosAsync(new GetToDosRequest());

foreach (var item in response.ToDos)
{
    Console.WriteLine($"{item.Id} - {item.Task}");
}


Console.WriteLine("Press any key to exit...");
Console.ReadKey();