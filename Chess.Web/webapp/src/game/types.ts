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
    squares: Square[],
}

export type GameState = {
    status: GameStatus,
    board: Board,
    currentTurn: Color
}

export type ChessEngineType = 
    "Stockfish" |
    "Deakfish"

export type GameConfig = {
    playerColor: Color,
    engineType: ChessEngineType,
    elo: number
}

export type ChessGameMode =
    "NormalPvE" |
    "NormalPvP" |
    "ClashPvE" |
    "ClashPvP" 

export type NormalPvEConfig = {
    playerColor: Color,
    engineType: ChessEngineType,
    elo: number,
    depth: number,
    moveTime: number,
    nodes: number
}

export type ClashPvEConfig = {
    playerColor: Color,
    engineType: ChessEngineType,
    elo: number,
    depth: number,
    moveTime: number,
    nodes: number,
    randomizeChessEnginePieces: boolean,
    playerPieces: PieceType[],
    chessEnginePieces: PieceType[] | null
}

export type NormalPvPConfig = {
    nickname: string
}

export type ClashPvPConfig = {
    nickname: string,
    playerPieces: PieceType[]
}

export type StartGameOptions = {
    sessionId: string | null,
    gameMode: ChessGameMode,
    normalPvEConfig: NormalPvEConfig | null,
    clashPvEConfig: ClashPvEConfig | null,
    normalPvPConfig: NormalPvPConfig | null,
    clashPvPConfig: ClashPvPConfig | null,
}