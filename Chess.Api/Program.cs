using Chess.Api.Game;
using Chess.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<GameSessionManager>();

var app = builder.Build();

app.MapHub<GameHub>("/gamehub");
app.Run();
