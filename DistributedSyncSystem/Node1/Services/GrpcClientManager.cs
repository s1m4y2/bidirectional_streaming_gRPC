using Common.Protos;
using Grpc.Core;
using Grpc.Net.Client;

namespace Node1.Services;

public class GrpcClientManager
{
    private readonly List<AsyncDuplexStreamingCall<DataUpdate, DataUpdate>> _activeStreams = new();

    public async Task ConnectToPeerAsync(string peerUrl)
    /*
     bu noktada sistem dinamik bir şekilde farklı node’larla iletişim kuruyor — yani peer-to-peer bir mimari kuruyorsun.
    peerUrl: Eş node’a bağlanmak için gereken adresi temsil eden değişken
    diğer tüm node'ların urllerini elle tek tek yazmak yerine peerUrl listesinde hepsi
     */
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
    /*
     birden fazla peer node’a veri yaymak (broadcast) için kullanılır.
    Bu bir asenkron metottur, Task döner çünkü içinde await kullanılır.

     */
    {
        foreach (var stream in _activeStreams) // _activeStreams: Aktif gRPC bağlantılarını (stream’lerini) tutan bir listedir.
        {
            try
            {
                await stream.RequestStream.WriteAsync(new DataUpdate
                {
                    Sender = sender,
                    Key = key,
                    Value = value
                });
                /*
                 veri, peer node’lara JSON değil, Protobuf binary formatında stream edilir.
                Bu çok hızlı ve az yer kaplayan bir aktarım şeklidir.
                 */
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veri gönderilemedi: {ex.Message}");
            }
        }
    }

}
