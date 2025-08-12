using Grpc.Core;
using GrpcChatApp;

namespace GrpcChatApp.Services;

public class ChatService : GrpcChatApp.ChatService.ChatServiceBase
// ChatService ad�nda bir s�n�f olu�turuyoruz. Ama dikkat! Bu s�n�f, gRPC taraf�ndan otomatik olarak olu�turulan ChatServiceBase soyut s�n�f�n� (abstract base class) miras al�yor.

{
    private static readonly List<IServerStreamWriter<ChatMessage>> _clients = new();
    // Bu liste, sunucuya ba�l� olan t�m client�lar�n responseStream nesnelerini tutar.
    // IServerStreamWriter<ChatMessage> tipindeki nesneler sayesinde sunucu, client�a mesaj g�nderebilir.
    // static oldu�u i�in t�m ba�lant�lar ayn� listeyi payla��r.

    public override async Task ChatStream(IAsyncStreamReader<ChatMessage> requestStream, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
    // gRPC sunucusundaki bidirectional streaming i�lemidir.
    // requestStream: client�tan gelen mesajlar� okur.
    // responseStream: server�dan client�a mesaj g�nderir.
    {
        lock (_clients)
        {
            _clients.Add(responseStream);
        }
        // Yeni bir client ba�land���nda, onun responseStream nesnesini _clients listesine ekliyoruz.
        // lock: �ok say�da client ayn� anda ba�lanabilir. lock sayesinde ayn� anda eri�im engelleniyor ve liste g�venli �ekilde g�ncelleniyor.
        try
        {
            await foreach (var message in requestStream.ReadAllAsync())
            // Bu, client�tan gelen mesajlar� tek tek s�rayla al�r.
            // ReadAllAsync(): s�rekli olarak client�tan gelen veriyi okur.
            // Bu d�ng� i�inde her mesaj i�lendi�i anda bir kere �al���r.
            {
                Console.WriteLine($"[{message.User}]: {message.Message}");

                // T�m kullan�c�lara mesaj� ilet
                List<Task> broadcastTasks;
                lock (_clients)
                {
                    broadcastTasks = _clients
                        .Where(client => client != responseStream) //Mesaj g�nderen ki�iye tekrar mesaj g�nderme 
                        .Select(client => client.WriteAsync(message)) //Her client�a mesaj� g�nder.
                        .ToList();
                }

                await Task.WhenAll(broadcastTasks);
                //T�m WriteAsync() i�lemleri e� zamanl� ba�lar ama hepsinin bitmesi beklenir.
            }
        }
        finally
        {
            lock (_clients)
            {
                _clients.Remove(responseStream);
            }
        }
    }
}
