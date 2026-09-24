using Chess.Api.Game;
using Chess.Api.Hubs;
using Chess.Api.Matchmaking;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration
	.GetSection("Cors:AllowedOrigins")
	.Get<string[]>() ?? [];

builder.Services.AddCors(options => options.AddPolicy("Frontend", policy =>
	policy.WithOrigins(allowedOrigins)
		.AllowAnyHeader()
		.AllowAnyMethod()
		.AllowCredentials()));

builder.Services.AddSignalR()
	.AddJsonProtocol(options =>
		options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// keep sessions and matchmaking in this server process; a restart clears them.
builder.Services.AddSingleton<GameSessionManager>();
builder.Services.AddSingleton<MatchmakingService>();

var app = builder.Build();

app.UseCors("Frontend");
app.MapHub<GameHub>("/gamehub");
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.Run();
