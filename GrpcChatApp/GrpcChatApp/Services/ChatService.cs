using Grpc.Core;
using GrpcChatApp;

namespace GrpcChatApp.Services;

public class ChatService : GrpcChatApp.ChatService.ChatServiceBase
// ChatService adýnda bir sýnýf oluþturuyoruz. Ama dikkat! Bu sýnýf, gRPC tarafýndan otomatik olarak oluþturulan ChatServiceBase soyut sýnýfýný (abstract base class) miras alýyor.

{
    private static readonly List<IServerStreamWriter<ChatMessage>> _clients = new();
    // Bu liste, sunucuya baðlý olan tüm client’larýn responseStream nesnelerini tutar.
    // IServerStreamWriter<ChatMessage> tipindeki nesneler sayesinde sunucu, client’a mesaj gönderebilir.
    // static olduðu için tüm baðlantýlar ayný listeyi paylaþýr.

    public override async Task ChatStream(IAsyncStreamReader<ChatMessage> requestStream, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
    // gRPC sunucusundaki bidirectional streaming iþlemidir.
    // requestStream: client’tan gelen mesajlarý okur.
    // responseStream: server’dan client’a mesaj gönderir.
    {
        lock (_clients)
        {
            _clients.Add(responseStream);
        }
        // Yeni bir client baðlandýðýnda, onun responseStream nesnesini _clients listesine ekliyoruz.
        // lock: Çok sayýda client ayný anda baðlanabilir. lock sayesinde ayný anda eriþim engelleniyor ve liste güvenli þekilde güncelleniyor.
        try
        {
            await foreach (var message in requestStream.ReadAllAsync())
            // Bu, client’tan gelen mesajlarý tek tek sýrayla alýr.
            // ReadAllAsync(): sürekli olarak client’tan gelen veriyi okur.
            // Bu döngü içinde her mesaj iþlendiði anda bir kere çalýþýr.
            {
                Console.WriteLine($"[{message.User}]: {message.Message}");

                // Tüm kullanýcýlara mesajý ilet
                List<Task> broadcastTasks;
                lock (_clients)
                {
                    broadcastTasks = _clients
                        .Where(client => client != responseStream) //Mesaj gönderen kiþiye tekrar mesaj gönderme 
                        .Select(client => client.WriteAsync(message)) //Her client’a mesajý gönder.
                        .ToList();
                }

                await Task.WhenAll(broadcastTasks);
                //Tüm WriteAsync() iþlemleri eþ zamanlý baþlar ama hepsinin bitmesi beklenir.
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
