using Common.Protos;
using Grpc.Core;

namespace Node2.Services; // Node3 için Node3.Services

public class SyncServiceImpl : SyncService.SyncServiceBase
{
    private static readonly List<IServerStreamWriter<DataUpdate>> _clients = new();

    public override async Task Sync(IAsyncStreamReader<DataUpdate> requestStream, IServerStreamWriter<DataUpdate> responseStream, ServerCallContext context)
    {
        _clients.Add(responseStream);

        try
        {
            await foreach (var update in requestStream.ReadAllAsync())
            {
                Console.WriteLine($"[GELEN] {update.Sender}: {update.Key} = {update.Value}");

                foreach (var client in _clients.Where(c => c != responseStream))
                {
                    await client.WriteAsync(update);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
        }
    }
}
