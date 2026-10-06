import type {HubConnection} from "@microsoft/signalr";
import {useEffect, useRef, useState} from "react";
import Board from "../components/Board.tsx";
import GameResultDialog from "../components/GameResultDialog.tsx";
import PromotionDialog from "../components/PromotionDialog.tsx";
import {cancelMatch, createConnection, findMatch, rejoinGame, resignGame, startGame, submitMove} from "../api/index.ts";
import {clearStoredGameSession, storeGameSession} from "../game/session.ts";
import type {
    Board as BoardState,
    ClashPvEConfig,
    Color,
    ErrorDto,
    GameConfig,
    GameStatus,
    MatchFoundDto,
    MoveRejectedDto,
    NormalPvEConfig,
    PieceType,
    Position,
    SnapshotDto
} from "../game/types.ts";

type PendingPromotion = {from: Position; to: Position};

function isPvEConfig(config: GameConfig): config is NormalPvEConfig | ClashPvEConfig {
    return config.gameMode === "NormalPvE" || config.gameMode === "ClashPvE";
}

export default function GamePage({config, initialToken, initialPlayerColor, onSessionSaved, onExit, onNewGame}: {
    config: GameConfig,
    initialToken?: string,
    initialPlayerColor?: Color,
    onSessionSaved: () => void,
    onExit: () => void,
    onNewGame: () => void
}) {
    const isPvE = isPvEConfig(config);
    const isMultiplayer = !isPvE;
    const [board, setBoard] = useState<BoardState>();
    const [status, setStatus] = useState<GameStatus>("InProgress");
    const [playerColor, setPlayerColor] = useState<Color | undefined>(initialPlayerColor ?? (isPvE ? config.playerColor : undefined));
    const [selected, setSelected] = useState<Position>();
    const [pendingPromotion, setPendingPromotion] = useState<PendingPromotion>();
    const [invalidMove, setInvalidMove] = useState<string>();
    const [isMatching, setIsMatching] = useState(isMultiplayer && !initialToken);
    const [hasToken, setHasToken] = useState(Boolean(initialToken));
    const connectionRef = useRef<HubConnection | null>(null);
    const tokenRef = useRef(initialToken);
    const invalidMoveTimerRef = useRef<number | undefined>(undefined);

    const gameEnded = status !== "InProgress";
    const resultText = status === "WhiteWonByCheckmate" ? "White wins by checkmate"
        : status === "BlackWonByCheckmate" ? "Black wins by checkmate"
        : status === "WhiteWonByResignation" ? "White wins by resignation"
        : status === "BlackWonByResignation" ? "Black wins by resignation"
        : gameEnded ? "Draw" : "";

    function showMessage(message: string) {
        setInvalidMove(message);
        window.clearTimeout(invalidMoveTimerRef.current);
        invalidMoveTimerRef.current = window.setTimeout(() => setInvalidMove(undefined), 3500);
    }

    useEffect(() => {
        const connection = createConnection();
        connectionRef.current = connection;

        function saveSession(token: string, color: Color) {
            tokenRef.current = token;
            setHasToken(true);
            setPlayerColor(color);
            setIsMatching(false);
            storeGameSession({config, token, playerColor: color});
            onSessionSaved();
        }

        connection.on("Snapshot", (snapshot: SnapshotDto) => {
            setBoard(snapshot.boardSquares.flat().map(square => ({
                color: square.color,
                position: {row: square.position.row, col: square.position.column},
                piece: square.piece
            })));
            setStatus(snapshot.status);
            if (snapshot.status !== "InProgress") clearStoredGameSession();
        });
        connection.on("MatchFound", (match: MatchFoundDto) => {
            if (match.matched && match.token) saveSession(match.token, match.playerColor);
        });
        connection.on("MoveRejected", (message: MoveRejectedDto) => showMessage(message.reason));
        connection.on("Error", (message: ErrorDto) => {
            if (message.message === "Game not found." && tokenRef.current) {
                tokenRef.current = undefined;
                setHasToken(false);
                clearStoredGameSession();
                onExit();
                return;
            }
            setIsMatching(false);
            showMessage(message.message);
        });

        async function joinCurrentSession() {
            if (tokenRef.current) {
                await rejoinGame(connection, tokenRef.current);
            } else if (isPvE) {
                const token = await startGame(connection, config);
                if (token) saveSession(token, config.playerColor);
            } else {
                setIsMatching(true);
                await findMatch(connection, config);
            }
        }

        connection.onreconnected(() => {
            void joinCurrentSession().catch(() => showMessage("Unable to reconnect to the game server."));
        });
        connection.start().then(joinCurrentSession).catch(() => showMessage("Unable to connect to the game server."));

        return () => {
            connection.off("Snapshot");
            connection.off("MatchFound");
            connection.off("MoveRejected");
            connection.off("Error");
            if (isMultiplayer && !tokenRef.current && connection.state === "Connected") void cancelMatch(connection);
            void connection.stop();
            window.clearTimeout(invalidMoveTimerRef.current);
        };
    }, [config, initialToken, isMultiplayer, isPvE, onExit, onSessionSaved]);

    async function exitGame() {
        const connection = connectionRef.current;
        if (gameEnded) {
            onExit();
            return;
        }

        if (isMatching && connection?.state === "Connected") {
            await cancelMatch(connection);
            onExit();
            return;
        }

        if (!tokenRef.current) {
            onExit();
            return;
        }

        if (!connection || connection.state !== "Connected") {
            showMessage("Connect to the game server before resigning.");
            return;
        }

        try {
            await resignGame(connection);
        } catch {
            showMessage("Unable to resign. Check the connection and try again.");
        }
    }

    function handleSquareClick(position: Position) {
        if (!board || !playerColor || status !== "InProgress") return;
        const square = board.find(item => item.position.row === position.row && item.position.col === position.col);
        if (!selected) {
            if (square?.piece?.color === playerColor) setSelected(position);
            return;
        }

        if (selected.row === position.row && selected.col === position.col) {
            setSelected(undefined);
            return;
        }

        if (square?.piece?.color === playerColor) {
            setSelected(position);
            return;
        }

        const selectedSquare = board.find(item => item.position.row === selected.row && item.position.col === selected.col);
        const promotionRow = playerColor === "White" ? 0 : 7;
        if (selectedSquare?.piece?.type === "Pawn" && position.row === promotionRow) {
            setPendingPromotion({from: selected, to: position});
            setSelected(undefined);
            return;
        }

        if (connectionRef.current?.state === "Connected") void submitMove(connectionRef.current, {from: selected, to: position});
        setSelected(undefined);
    }

    function promotePiece(piece: PieceType) {
        if (!pendingPromotion || connectionRef.current?.state !== "Connected") return;
        void submitMove(connectionRef.current, {...pendingPromotion, promotion: piece});
        setPendingPromotion(undefined);
    }

    function playerName(color: Color) {
        if (isPvE) return color === config.playerColor ? "You" : config.chessEngineType;
        return color === playerColor ? config.nickname : "Opponent";
    }

    const topColor = playerColor === "White" ? "Black" : "White";

    return <main className="relative flex min-h-screen items-center justify-center overflow-hidden bg-[#17221f] p-4 text-[#f5ecd9] sm:p-6">
        <div className="fixed right-4 top-4 z-20 flex flex-col items-end gap-2">
            {!gameEnded && <button className="border border-[#f1dfc1] bg-[#f1dfc1] px-4 py-2 text-sm font-semibold text-[#3d2b24] hover:bg-[#9b6048] hover:text-[#f1dfc1]"
                type="button" onClick={() => void exitGame()}>
                {!hasToken ? "Back home" : isMatching ? "Cancel search" : "Resign"}
            </button>}
        </div>
        <section className="relative w-[min(92vw,calc(100dvh-8rem))] max-w-[44rem]">
            {playerColor && <div className="mb-2 flex items-center justify-between gap-3 text-sm font-semibold" aria-live="polite">
                <span>{playerName(topColor)}</span>
                {isMultiplayer && <span className="text-xs font-normal text-[#c4b9a6]">{topColor}</span>}
            </div>}
            {board && playerColor
                ? <Board board={board} playerColor={playerColor} selected={selected} onSquareClick={handleSquareClick}/>
                : <div className="flex aspect-square w-full items-center justify-center bg-[#20302b] text-sm text-[#d5c2a5]">
                    {isMatching ? "Searching for an opponent..." : invalidMove ?? "Connecting to game..."}
                </div>}
            {playerColor && <div className="mt-2 flex items-center justify-between gap-3 text-sm font-semibold">
                <span>{playerName(playerColor)}</span>
                {isMultiplayer && <span className="text-xs font-normal text-[#c4b9a6]">{playerColor}</span>}
            </div>}
            {invalidMove && <div className="absolute bottom-4 left-1/2 z-10 flex w-[min(19rem,calc(100%-2rem))] -translate-x-1/2 items-center justify-between gap-4 border border-[#888] bg-[#292929] px-4 py-3 text-sm text-[#f1f1f1] shadow-lg" role="alert" aria-live="assertive">
                <span>{invalidMove}</span>
                <button className="text-lg leading-none text-[#cfcfcf] hover:text-white" type="button" aria-label="Dismiss warning" onClick={() => setInvalidMove(undefined)}>x</button>
            </div>}
        </section>
        {pendingPromotion && <PromotionDialog onPromote={promotePiece} onCancel={() => setPendingPromotion(undefined)} />}
        {gameEnded && <GameResultDialog result={resultText} onNewGame={onNewGame} onHome={onExit} />}
    </main>;
}