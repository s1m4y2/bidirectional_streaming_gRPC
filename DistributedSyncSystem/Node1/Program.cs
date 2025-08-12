using Node1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();
app.MapGrpcService<Node1.Services.SyncServiceImpl>();
app.MapGet("/", () => "Node1 çalışıyor!");

var manager = new GrpcClientManager();
app.Lifetime.ApplicationStarted.Register(async () =>
{
    await manager.ConnectToPeerAsync("http://localhost:5002");
    await manager.ConnectToPeerAsync("http://localhost:5003");

    // Bağlantı kurulduktan sonra veri gönder
    await Task.Delay(1000); // 1 saniye beklet 
    await manager.BroadcastUpdateAsync("Node1", "Status", "Ready");
});

var connectionString = "Server=localhost;Database=DistributedSyncDB;";
var nodeName = "node1"; // Bu node kendisini "node1" olarak tanımlıyor




app.Run();
