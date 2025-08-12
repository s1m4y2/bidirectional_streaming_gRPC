using GrpcChatApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5235); // HTTP
    options.ListenAnyIP(7226, listenOptions =>
    {
        listenOptions.UseHttps(); // HTTPS (sertifika gerektirir)
    });
});


var app = builder.Build();

app.MapGrpcService<ChatService>();
app.MapGet("/", () => "gRPC Chat Server çalýþýyor!");

app.Run();
