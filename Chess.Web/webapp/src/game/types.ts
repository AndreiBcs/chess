export type GameStatus = 
    "InProgress" |
    "WhiteWon" |
    "BlackWon" |
    "DrawByStalemate" |
    "DrawByThreefoldRepetition" |
    "DrawByInsufficientMaterial" |
    "DrawBy75MoveRule"

export type Color =
    "White" |
    "Black"

export type PieceType =
    "Pawn" |
    "Rook" |
    "Knight" |
    "Bishop" |
    "Queen" |
    "King"

export type Piece = {
    color: Color, 
    type: PieceType,
    filePath: string,
    letterId: string
}

export type Position = {
    row: number,
    col: number
}

export type Move = {
    from: Position,
    to: Position,
    promotion?: PieceType
}

export type Square = {
    color: Color,
    position: Position,
    piece: Piece | null
}

export type Board = {
    squares: Square[][],
}

export type GameState = {
    status: GameStatus,
    board: Board,
    currentTurn: Color
}

export type EngineType = 
    "Stockfish"

export type GameConfig = {
    playerColor: Color,
    engineType: EngineType,
    elo: number
}