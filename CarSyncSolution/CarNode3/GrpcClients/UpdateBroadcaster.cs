namespace CarNode3.GrpcClients
{
    using Grpc.Net.Client;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Concurrent;
    using System.Threading.Channels;
    using CarNode3.Data;
    using Common;
    using Grpc.Core;

    public class UpdateBroadcaster
    {
        private readonly List<string> _otherNodeAddresses;
        private readonly string _currentCarId;
        private readonly AppDbContext _dbContext;

        public UpdateBroadcaster(List<string> otherNodeAddresses, string currentCarId, AppDbContext dbContext)
        {
            _otherNodeAddresses = otherNodeAddresses;
            _currentCarId = currentCarId;
            _dbContext = dbContext;
        }

        public async Task StartAsync()
        {
            var tasks = _otherNodeAddresses.Select(address => ConnectToNodeAsync(address));
            await Task.WhenAll(tasks);
        }

        private async Task ConnectToNodeAsync(string address)
        {
            using var channel = GrpcChannel.ForAddress(address);
            var client = new CarSync.CarSyncClient(channel);
            using var call = client.SyncUpdates();

            // Mesaj gönderme
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    // Burada örnek olarak veritabanından car1'in status bilgisi alınır
                    var car = await _dbContext.Cars.FirstOrDefaultAsync(c => c.CarId == _currentCarId);

                    var update = new CarUpdate
                    {
                        CarId = car.CarId,
                        Status = car.Status,
                        UpdatedField = "Status",
                        NewValue = car.Status
                    };

                    await call.RequestStream.WriteAsync(update);
                    Console.WriteLine($"[Client] Sent update to {address}");
                    await Task.Delay(5000); // 5 sn'de bir güncelleme gönder
                }
            });

            // Mesaj alma
            await foreach (var update in call.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"[Client] Received update from {update.CarId}: {update.UpdatedField} -> {update.NewValue}");

                // Eğer güncelleme bu node'a ait değilse veritabanını güncelle
                if (update.CarId != _currentCarId)
                {
                    var car = await _dbContext.Cars.FirstOrDefaultAsync(c => c.CarId == update.CarId);
                    if (car != null)
                    {
                        car.Status = update.NewValue;
                        await _dbContext.SaveChangesAsync();
                        Console.WriteLine($"[Client] Updated local DB for {car.CarId}");
                    }
                }
            }
        }
    }

}
