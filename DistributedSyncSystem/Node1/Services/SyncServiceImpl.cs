using Common.Protos;
using Grpc.Core;

namespace Node1.Services;

public class SyncServiceImpl : SyncService.SyncServiceBase
{
    private static readonly List<IServerStreamWriter<DataUpdate>> _clients = new();
    /*
     List<>: Tüm baðlý client’larýn çýktýsýný (stream’ini) saklayan liste.
    static: Tüm baðlantýlar bu listeyi paylaþýr. Yani tüm client’lar arasýnda ortak bir kayýt defteri gibi çalýþýr.
    Amaç: Yeni biri baðlandýðýnda onun kanalýný bu listeye ekleyip diðerlerine mesaj gönderebilmek.
     */

    public override async Task Sync(IAsyncStreamReader<DataUpdate> requestStream, IServerStreamWriter<DataUpdate> responseStream, ServerCallContext context)
    {
        _clients.Add(responseStream); //Yeni baðlanan client’ýn responseStream kanalýný _clients listesine ekliyoruz.

        try
        {
            await foreach (var update in requestStream.ReadAllAsync())
            {
                Console.WriteLine($"[GELEN] {update.Sender}: {update.Key} = {update.Value}");

                foreach (var client in _clients.Where(c => c != responseStream)) //Bu döngü, gelen mesajý gönderen hariç diðer tüm client’lara iletir.
                //c != responseStream: Göndereni tekrar bilgilendirmiyoruz.
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
