using Chess.Api.Game;
using Chess.Api.Hubs;
using Chess.Api.Matchmaking;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR()
	.AddJsonProtocol(options =>
		options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// everything is in memory
builder.Services.AddSingleton<GameSessionManager>();
builder.Services.AddSingleton<MatchmakingService>();

var app = builder.Build();

app.MapHub<GameHub>("/gamehub");
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.Run();
