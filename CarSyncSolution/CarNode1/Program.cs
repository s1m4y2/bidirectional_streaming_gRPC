using CarNode1.Services;
using Microsoft.EntityFrameworkCore;
using CarNode1.Data;
using CarNode1.GrpcClients;

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
            "https://localhost:5002", // CarNode2
            "https://localhost:5003"  // CarNode3
        },
        currentCarId: "car1",
        dbContext: dbContext
    );

    _ = broadcaster.StartAsync();
}

app.Run();