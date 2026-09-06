using Chess.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddControllers();

builder.Services.AddSingleton<GameHub>();

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseWebSockets();
app.MapControllers();
app.Run();
