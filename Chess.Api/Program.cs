using Chess.Api.Hubs;
using Microsoft.AspNetCore.WebSockets;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddControllers();

builder.Services.AddSingleton<GameHub>();
builder.Services.AddWebSockets(new WebSocketOptions 
{
    KeepAliveInterval = TimeSpan.FromSeconds(30)
});

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseWebSockets();
app.Run();
