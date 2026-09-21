using chess;
using Chess.Engine;

namespace Chess.Cli.Arguments;

public sealed record Options(
    Color PlayerColor, 
    ChessEngine Engine, 
    int Elo,
    int Depth,
    int MoveTime,
    long Nodes,
    bool TextRender);