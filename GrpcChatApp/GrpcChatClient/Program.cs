using Grpc.Core;
using Grpc.Net.Client;
using GrpcChatApp;

Console.Write("İsminizi girin: ");
string user = Console.ReadLine();

using var channel = GrpcChannel.ForAddress("https://localhost:7226");
var client = new ChatService.ChatServiceClient(channel);

using var call = client.ChatStream();

var receiveTask = Task.Run(async () =>
{
    await foreach (var response in call.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine($"\n{response.User}: {response.Message}");
    }
});

while (true)
{
    var message = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(message)) continue;

    await call.RequestStream.WriteAsync(new ChatMessage
    {
        User = user,
        Message = message
    });

    if (message.ToLower() == "bye")
    {
        await call.RequestStream.CompleteAsync();
        break;
    }
}

await receiveTask;
