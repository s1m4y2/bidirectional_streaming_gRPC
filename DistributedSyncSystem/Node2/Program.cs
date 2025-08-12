using Node2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<Node2.Services.SyncServiceImpl>();
app.MapGet("/", () => "Node çalışıyor!");

var manager = new Node2.Services.GrpcClientManager();
app.Lifetime.ApplicationStarted.Register(async () =>
{
    await manager.ConnectToPeerAsync("http://localhost:5001");
    await manager.ConnectToPeerAsync("http://localhost:5003");

    // Bağlantı kurulduktan sonra veri gönder
    await Task.Delay(1000); // 1 saniye beklet (isteğe bağlı)
    await manager.BroadcastUpdateAsync("Node2", "Status", "Ready");
});

var connectionString = "Server=localhost;Database=DistributedSyncDB;";
var nodeName = "node2"; 



app.Run();
