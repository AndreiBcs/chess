export type GameStatus =
    "InProgress" |
    "WhiteWonByCheckmate" |
    "BlackWonByCheckmate" |
    "WhiteWonByResignation" |
    "BlackWonByResignation" |
    "DrawByStalemate" |
    "DrawByInsufficientMaterial" |
    "DrawByThreefoldRepetition" |
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
    hasMoved: boolean,
    letterId: string
}

// Internal board positions use col; the API's Position DTO uses column.
export type Position = {
    row: number,
    col: number
}

export type PositionDto = {
    row: number,
    column: number
}

export type Move = {
    from: Position,
    to: Position,
    promotion?: PieceType | null
}

export type Square = {
    color: Color,
    position: Position,
    piece: Piece | null
}

export type Board = Square[]

export type SnapshotDto = {
    boardSquares: Array<Array<Omit<Square, "position"> & {position: PositionDto}>>,
    status: GameStatus,
    currentTurn: Color
}

export type MatchFoundDto = {
    matched: boolean,
    token: string | null,
    playerColor: Color
}

export type ErrorDto = {message: string}
export type MoveRejectedDto = {reason: string}

export type GameState = {
    status: GameStatus,
    board: Board,
    currentTurn: Color
}

export type ChessEngineType =
    "Stockfish" |
    "Deakfish"

export type ChessGameMode =
    "NormalPvE" |
    "NormalPvP" |
    "ClashPvE" |
    "ClashPvP" 

export type NormalPvEConfig = {
    type: "normalPvE",
    gameMode: "NormalPvE",
    playerColor: Color,
    chessEngineType: ChessEngineType,
    elo: number,
    depth: number,
    moveTime: number,
    nodes: number
}

export type ClashPvEConfig = {
    type: "clashPvE",
    gameMode: "ClashPvE",
    playerColor: Color,
    chessEngineType: ChessEngineType,
    elo: number,
    depth: number,
    moveTime: number,
    nodes: number,
    randomizeChessEnginePieces: boolean,
    playerPieces: PieceType[],
    chessEnginePieces: PieceType[] | null
}

export type NormalPvPConfig = {
    type: "normalPvP",
    gameMode: "NormalPvP",
    nickname: string
}

export type ClashPvPConfig = {
    type: "clashPvP",
    gameMode: "ClashPvP",
    nickname: string,
    playerPieces: PieceType[]
}

export type StartGameOptions = NormalPvEConfig | ClashPvEConfig | NormalPvPConfig | ClashPvPConfig
export type GameConfig = StartGameOptions