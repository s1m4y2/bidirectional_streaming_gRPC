using CarNode3.Services;
using Microsoft.EntityFrameworkCore;
using CarNode3.Data;
using CarNode3.GrpcClients;

var builder = WebApplication.CreateBuilder(args);

// EF MSSQL baðlama
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// gRPC servisini ekle
builder.Services.AddGrpc();

builder.Services.AddScoped<CarSyncService>();

var app = builder.Build();

app.MapGrpcService<CarSyncService>();
app.MapGet("/", () => "CarSync gRPC server is running...");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // CarNode1 kendini "car1" olarak tanýmlýyor
    var broadcaster = new UpdateBroadcaster(
        otherNodeAddresses: new List<string>
        {
            "https://localhost:5001", 
            "https://localhost:5002"  
        },
        currentCarId: "car3",
        dbContext: dbContext
    );

    _ = broadcaster.StartAsync();
}

app.Run();