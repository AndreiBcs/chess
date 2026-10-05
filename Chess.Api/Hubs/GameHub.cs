using System.Collections.Concurrent;
using chess;
using Chess.Api.Dtos.ClientDtos;
using Chess.Api.Dtos.ServerDtos;
using Chess.Api.Game;
using chess.Moves;
using Microsoft.AspNetCore.SignalR;

namespace Chess.Api.Hubs;

public sealed class GameHub(IHubContext<GameHub> ctx) : Hub
{
    // map id - session
    private static readonly ConcurrentDictionary<string, GameSession> Sessions = new();
    // map game mode - id and start options
    private static readonly Dictionary<ChessGameMode, (string Conn, StartGameDto Dto)> Waiting = new();
    private static readonly Lock MatchLock = new();

    private static GameSession? FindSession(string connId) =>
        Sessions.Values.FirstOrDefault(s => s.HasConnection(connId));

    public override Task OnDisconnectedAsync(Exception? ex)
    {
        lock (MatchLock)
        {
            // remove waiting connections if disconnected
            foreach (var mode in Waiting
                         .Where(w => w.Value.Conn == Context.ConnectionId)
                         .Select(w => w.Key)
                         .ToList())
            {
                Waiting.Remove(mode);
            }
        }

        // remove active sessions if disconnected
        var session = FindSession(Context.ConnectionId);
        
        if (session is not null)
        {
            session.Leave(Context.ConnectionId);
            if (!session.HasConnections)
            {
                // wait 10 mins before cancel in case of reconnect
                _ = CancelAfterGraceAsync(session);
            }
        }
        
        return base.OnDisconnectedAsync(ex);
    }

    private static async Task CancelAfterGraceAsync(GameSession s)
    {
        // after 10 minutes check if the session still has connections and cancel if not
        await Task.Delay(TimeSpan.FromMinutes(10));
        
        if (s.IdleSince is { } t && 
            DateTime.UtcNow - t >= TimeSpan.FromMinutes(10))
        {
            s.Cancel();
        }
    }

    public async Task<string?> StartGame(StartGameDto dto)
    {
        try
        {
            if (Sessions.Count >= 100)
            {
                throw new InvalidOperationException("Server full. Try again later.");
            }
            
            if (FindSession(Context.ConnectionId) is not null)
            {
                throw new InvalidOperationException("Already bound to a session.");   
            }
            
            dto.Validate();

            if (ModeOf(dto) != dto.GameMode)
            {
                throw new InvalidOperationException("Invalid game mode.");
            }
            
            if (dto.GameMode is not (ChessGameMode.NormalPvE or ChessGameMode.ClashPvE))
            {
                throw new InvalidOperationException("Use FindMatch for multiplayer modes.");
            }

            var id = Guid.NewGuid().ToString("N");
            var session = GameSession.CreatePvE(
                id,
                dto,
                ctx,
                sid => Sessions.TryRemove(sid, out _));
            
            Sessions[id] = session;

            session.Join(Context.ConnectionId, session.SoleClientColor!.Value);
            await Groups.AddToGroupAsync(Context.ConnectionId, id);
            session.Start();
            
            return session.TokenFor(session.SoleClientColor!.Value);
        }
        catch (Exception ex)
        {
            await Send("Error", new ErrorDto(ex.Message)); 
            return null; 
        }
    }

    public async Task Rejoin(string token)
    {
        try
        {
            GameSession? session = null;
            Color color = default;

            foreach (var s in Sessions.Values)
            {
                if (s.TryGetColor(token, out color))
                {
                    session = s;
                    break;
                }
            }
            
            if (session is null)
                throw new InvalidOperationException("Game not found.");

            session.Join(Context.ConnectionId, color);
            await Groups.AddToGroupAsync(Context.ConnectionId, session.SessionId);
            
            if (session.LastSnapshot is not null)
            {
                await Send("Snapshot", GameSnapshotDto.ToSnapshotDto(session.LastSnapshot));
            }
        }
        catch (Exception ex)
        {
            await Send("Error", new ErrorDto(ex.Message));
        }
    }

    public async Task FindMatch(StartGameDto dto)
    {
        try
        {
            if (Sessions.Count >= 100)
            {
                throw new InvalidOperationException("Server full. Try again later.");
            }
            
            if (FindSession(Context.ConnectionId) is not null)
            {
                throw new InvalidOperationException("Already bound to a session.");   
            }
            
            dto.Validate();

            if (ModeOf(dto) != dto.GameMode)
            {
                throw new InvalidOperationException("Invalid game mode.");
            }
            
            if (dto.GameMode is not (ChessGameMode.NormalPvP or ChessGameMode.ClashPvP))
            {
                throw new InvalidOperationException("Not a multiplayer mode.");
            }

            (string Conn, StartGameDto Dto)? opponent = null;
            
            lock (MatchLock)
            {
                foreach (var m in Waiting
                             .Where(pair => pair.Value.Conn == Context.ConnectionId 
                                            && pair.Key != dto.GameMode)
                             .Select(pair => pair.Key)
                             .ToList())
                {
                    Waiting.Remove(m);
                }
                
                if (Waiting.Remove(dto.GameMode, out var w) &&
                    w.Conn != Context.ConnectionId)
                {
                    opponent = w;
                }
                else
                {
                    Waiting[dto.GameMode] = (Context.ConnectionId, dto);
                }
            }

            if (opponent is not { } opp) 
                return;

            var me = (Conn: Context.ConnectionId, Dto: dto);
            var (white, black) = Random.Shared.Next(2) == 0 ? (me, opp) : (opp, me);

            var id = Guid.NewGuid().ToString("N");
            var session = GameSession.CreatePvP(
                id,
                dto.GameMode,
                GameSession.CreatePlayer(Color.White, white.Dto),
                GameSession.CreatePlayer(Color.Black, black.Dto),
                ctx,
                sid => Sessions.TryRemove(sid, out _));

            Sessions[id] = session;

            foreach (var (conn, color) in new[] { (white.Conn, Color.White), (black.Conn, Color.Black) })
            {
                session.Join(conn, color);
                await ctx.Groups.AddToGroupAsync(conn, id);
                await ctx.Clients
                    .Client(conn)
                    .SendAsync("MatchFound", new MatchmakingDto(true, session.TokenFor(color), color));
            }

            session.Start();
        }
        catch (Exception ex)
        {
            await Send("Error", new ErrorDto(ex.Message));
        }
    }

    public void CancelMatch()
    {
        lock (MatchLock)
        {
            foreach (var mode in Waiting
                         .Where(w => w.Value.Conn == Context.ConnectionId)
                         .Select(w => w.Key)
                         .ToList())
            {
                Waiting.Remove(mode);
            }
        }
    }

    public async Task ResignGame()
    {
        try
        {
            var s = FindSession(Context.ConnectionId);
            
            if (s is null)
            {
                throw new InvalidOperationException("No active game for this connection.");
            }
            
            await s.ResignAsync(Context.ConnectionId);
        }
        catch (Exception ex)
        {
            await Send("Error", new ErrorDto(ex.Message));
        }
    }

    public async Task SubmitMove(Move move)
    {
        try
        {
            var session = FindSession(Context.ConnectionId)
                          ?? throw new InvalidOperationException("No active game for this connection.");
            session.SubmitMove(Context.ConnectionId, move);
        }
        catch (Exception ex)
        {
            await Send("Error", new ErrorDto(ex.Message));
        }
    }

    private Task Send(string method, object payload) => Clients.Caller.SendAsync(method, payload);
    
    private static ChessGameMode ModeOf(StartGameDto d) => d switch
    {
        NormalPvEStartDto => ChessGameMode.NormalPvE,
        ClashPvEStartDto  => ChessGameMode.ClashPvE,
        NormalPvPStartDto => ChessGameMode.NormalPvP,
        ClashPvPStartDto  => ChessGameMode.ClashPvP,
        _ => throw new InvalidDataException("Unknown start request.")
    };
}