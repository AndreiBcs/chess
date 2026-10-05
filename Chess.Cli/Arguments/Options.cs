using chess;
using Chess.Engine;

namespace Chess.Cli.Arguments;

public sealed record Options(
    Color PlayerColor, 
    ChessEngineType EngineType, 
    int Elo,
    int Depth,
    int MoveTime,
    long Nodes,
    bool TextRender);