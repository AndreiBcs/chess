using chess;
using Chess.Api.Dtos.ClientDtos;
using Chess.Api.Hubs;
using Chess.Api.Player;
using Chess.Engine;
using chess.Pieces;
using Microsoft.AspNetCore.SignalR;

namespace Chess.Api.Game;

public sealed partial class GameSession
{
    private readonly EnginePlayer? _engine;
    private readonly int? _elo;
    public Color? SoleClientColor => _clients.Count == 1 ? _clients.Keys.First() : null;
    
    public static GameSession CreatePvE(
        string id,
        StartGameDto dto,
        IHubContext<GameHub> hub,
        Action<string> onFinished) => dto switch
    {
        NormalPvEStartDto n => BuildPvE(
            id,
            n.GameMode,
            n.PlayerColor,
            n.ChessEngineType,
            n.Elo,
            n.Depth,
            n.MoveTime,
            n.Nodes,
            null,
            null,
            null,
            hub,
            onFinished),
        
        ClashPvEStartDto c => BuildPvE(
            id,
            c.GameMode,
            c.PlayerColor,
            c.ChessEngineType,
            c.Elo,
            c.Depth,
            c.MoveTime,
            c.Nodes,
            c.PlayerPieces,
            c.ChessEnginePieces,
            c.RandomizeChessEnginePieces,
            hub,
            onFinished),
        
        _ => throw new ArgumentException("Not a PvE start request.")
    };

    private static GameSession BuildPvE(
        string id,
        ChessGameMode mode,
        Color color,
        ChessEngineType engineType,
        int elo,
        int depth,
        int moveTime,
        long nodes,
        PieceType[]? playerPieces,
        PieceType[]? enginePieces,
        bool? randomize,
        IHubContext<GameHub> hub,
        Action<string> onFinished)
    {
        var human = new HttpPlayer(color);
        var oppositeColor = color == Color.White ? Color.Black : Color.White;
        var engine = new EnginePlayer(oppositeColor, engineType);
        engine.Uci.Depth = depth;
        engine.Uci.MoveTime = moveTime;
        engine.Uci.Nodes = nodes;

        if (mode == ChessGameMode.ClashPvE)
        {
            human.ClashPieces = playerPieces!.ToList();
            engine.ClashPieces = randomize!.Value
                ? RandomClashSetup()
                : enginePieces!.ToList();
        }

        var game = new chess.Game.Game(human, engine, mode);
        
        return new GameSession(
            id,
            game,
            new Dictionary<Color, HttpPlayer> { [color] = human },
            engine,
            elo,
            hub,
            onFinished);
    }

    private static List<PieceType> RandomClashSetup()
    {
        throw new NotImplementedException();
    }
}