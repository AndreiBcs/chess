using Chess.Api.Hubs;
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
	{
		options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
		options.PayloadSerializerOptions.AllowOutOfOrderMetadataProperties = true;
	});

var app = builder.Build();

app.UseCors("Frontend");
app.MapHub<GameHub>("/gamehub");
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.Run();
