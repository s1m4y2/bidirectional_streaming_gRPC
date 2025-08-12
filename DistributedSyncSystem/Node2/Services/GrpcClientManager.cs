using Common.Protos;
using Grpc.Net.Client;
using Grpc.Core;

namespace Node2.Services; 

public class GrpcClientManager
{
    private readonly List<AsyncDuplexStreamingCall<DataUpdate, DataUpdate>> _activeStreams = new();

    public async Task ConnectToPeerAsync(string peerUrl)
    {
        try
        {
            var channel = GrpcChannel.ForAddress(peerUrl);
            var client = new SyncService.SyncServiceClient(channel);
            var call = client.Sync();

            _activeStreams.Add(call); // Sadece bağlantı başarılıysa ekliyoruz

            _ = Task.Run(async () =>
            {
                await foreach (var update in call.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine($"[GÜNCELLEME] {update.Sender}: {update.Key} = {update.Value}");
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Peer'e bağlanılamadı: {peerUrl} - {ex.Message}");
        }
    }


    public async Task BroadcastUpdateAsync(string sender, string key, string value)
    {
        foreach (var stream in _activeStreams)
        {
            try
            {
                await stream.RequestStream.WriteAsync(new DataUpdate
                {
                    Sender = sender,
                    Key = key,
                    Value = value
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veri gönderilemedi: {ex.Message}");
            }
        }
    }

}
