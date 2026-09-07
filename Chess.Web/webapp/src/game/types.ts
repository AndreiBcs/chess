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

export type Square = {
    color: Color,
    position: Position,
    piece?: Piece
}

export type Board = {
    squares: Square[][],
}

export type GameState = {
    status: GameStatus,
    board: Board,
    currentTurn: Color
}