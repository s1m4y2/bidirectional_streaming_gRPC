using Grpc.Core;
using CarNode2;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Common;
using CarNode2.Data;

namespace CarNode2.Services;

public class CarSyncService : CarSync.CarSyncBase
{
    private readonly AppDbContext _dbContext;

    public CarSyncService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task SyncUpdates(
        IAsyncStreamReader<CarUpdate> requestStream,
        IServerStreamWriter<CarUpdate> responseStream,
        ServerCallContext context)
    {
        // Gelen stream'den veri okuyalým
        await foreach (var update in requestStream.ReadAllAsync())
        {
            // Eðer gelen update bu node'a ait deðilse DB'yi güncelle
            var car = await _dbContext.Cars.FirstOrDefaultAsync(c => c.CarId == update.CarId);

            if (car != null)
            {
                car.Status = update.NewValue;
                await _dbContext.SaveChangesAsync();
                Console.WriteLine($"[Server] Updated DB: {car.CarId} -> {car.Status}");

                // Ýstersen burada responseStream.WriteAsync ile
                // ayný update’i client’lara da iletebilirsin
                // (örnek olarak aþaðýdaki kodu açabilirsin)
                // await responseStream.WriteAsync(update);
            }
            else
            {
                Console.WriteLine($"[Server] Received update for unknown carId: {update.CarId}");
            }
        }
    }
}


