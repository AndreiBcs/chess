using Chess.Api.Dtos.RequestDtos;
using Chess.Api.Dtos.ResponseDtos;
using Chess.Api.Game;
using chess.Game;
using chess.Moves;
using Microsoft.AspNetCore.SignalR;

namespace Chess.Api.Hubs;

public sealed class GameHub : Hub
{
    private const string SessionCookieName = "chess_session_id";
    private readonly GameSessionManager _sessionManager;

    public GameHub(GameSessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _sessionManager.RemoveConnection(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task StartGame(StartRequestDto request)
    {
        try
        {
            StartRequestDto.Validate(request);

            var sessionId = ResolveSessionId(request.GameId);
            var session = _sessionManager.GetSession(sessionId);

            if (session is null)
            {
                session = _sessionManager.CreateSession(sessionId, request);
                await session.StartAsync();
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
            _sessionManager.RegisterConnection(Context.ConnectionId, sessionId);
            SetSessionCookie(sessionId);

            if (session.LastSnapshot is not null)
            {
                await Clients.Caller.SendAsync(
                    "ReceiveMessage",
                    SnapshotDto.ToSnapshotDto(session.LastSnapshot));
            }
        }
        catch (Exception ex)
        {
            await SendErrorAsync(ex.Message);
        }
    }

    public async Task SubmitMove(Move move)
    {
        try
        {
            var sessionId = _sessionManager.GetSessionIdForConnection(Context.ConnectionId);
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                await SendErrorAsync("No active game session for this connection.");
                return;
            }

            var session = _sessionManager.GetSession(sessionId);
            if (session is null)
            {
                await SendErrorAsync("Game session not found.");
                return;
            }

            if (session.LastSnapshot is not null && session.LastSnapshot.Status != GameStatus.InProgress)
            {
                await SendErrorAsync("The game is already over.");
                return;
            }

            session.ProvideMoveFromClient(move);
        }
        catch (Exception ex)
        {
            await SendErrorAsync(ex.Message);
        }
    }

    private string ResolveSessionId(string? providedSessionId)
    {
        var httpContext = Context.GetHttpContext();
        var cookieSessionId = httpContext?.Request.Cookies[SessionCookieName];

        if (!string.IsNullOrWhiteSpace(providedSessionId))
        {
            return providedSessionId;
        }

        if (!string.IsNullOrWhiteSpace(cookieSessionId))
        {
            return cookieSessionId;
        }

        return Guid.NewGuid().ToString("N");
    }

    private void SetSessionCookie(string sessionId)
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext is null)
        {
            return;
        }

        httpContext.Response.Cookies.Append(SessionCookieName, sessionId, new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Secure = true,
            Expires = DateTimeOffset.UtcNow.AddMinutes(5)
        });
    }

    private async Task SendErrorAsync(string message)
    {
        await Clients.Caller.SendAsync("ReceiveMessage", message);
    }
}