using Common.Protos;
using Grpc.Core;

namespace Node1.Services;

public class SyncServiceImpl : SyncService.SyncServiceBase
{
    private static readonly List<IServerStreamWriter<DataUpdate>> _clients = new();
    /*
     List<>: T�m ba�l� client�lar�n ��kt�s�n� (stream�ini) saklayan liste.
    static: T�m ba�lant�lar bu listeyi payla��r. Yani t�m client�lar aras�nda ortak bir kay�t defteri gibi �al���r.
    Ama�: Yeni biri ba�land���nda onun kanal�n� bu listeye ekleyip di�erlerine mesaj g�nderebilmek.
     */

    public override async Task Sync(IAsyncStreamReader<DataUpdate> requestStream, IServerStreamWriter<DataUpdate> responseStream, ServerCallContext context)
    {
        _clients.Add(responseStream); //Yeni ba�lanan client��n responseStream kanal�n� _clients listesine ekliyoruz.

        try
        {
            await foreach (var update in requestStream.ReadAllAsync())
            {
                Console.WriteLine($"[GELEN] {update.Sender}: {update.Key} = {update.Value}");

                foreach (var client in _clients.Where(c => c != responseStream)) //Bu d�ng�, gelen mesaj� g�nderen hari� di�er t�m client�lara iletir.
                //c != responseStream: G�ndereni tekrar bilgilendirmiyoruz.
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
