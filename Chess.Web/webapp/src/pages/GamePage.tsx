import {createConnection, startGame, submitMove} from "../api/index.ts"
import type { HubConnection } from "@microsoft/signalr";
import {useEffect, useRef, useState} from "react";
import Board from "../components/Board.tsx";
import type {GameConfig, Board as BoardState, GameStatus, PieceType, Position} from "../game/types.ts";

type PendingPromotion = {
    from: Position;
    to: Position;
};

type ApiMessage = Record<string, unknown>;

function isSnapshot(message: unknown): message is ApiMessage & {
    boardSquares: Array<Array<ApiMessage & {position: {row: number, column: number}, piece: ApiMessage | null}>>;
    status: string | number;
} {
    return typeof message === "object" && message !== null && "boardSquares" in message && "status" in message;
}

function isMoveStatus(message: unknown): message is ApiMessage & {resultReason: string} {
    return typeof message === "object" && message !== null && "resultReason" in message;
}

function messageValue(message: ApiMessage, key: string) {
    return message[key] as string | number;
}

function enumName<const T extends string>(value: string | number, names: readonly T[]): T {
    return (typeof value === "number" ? names[value] : value) as T;
}

export default function GamePage ({config, gameId, onExit}: {config: GameConfig, gameId: string, onExit: () => void}) {
    const [board, setBoard] = useState<BoardState>();
    const [status, setStatus] = useState<GameStatus>("InProgress");
    const [selected, setSelected] = useState<Position>();
    const [pendingPromotion, setPendingPromotion] = useState<PendingPromotion>();
    const [invalidMove, setInvalidMove] = useState<string>();
    const connectionRef = useRef<HubConnection | null>(null);
    const invalidMoveTimerRef = useRef<number | undefined>(undefined);

    const gameEnded = status !== "InProgress";
    const resultText = status === "WhiteWon"
        ? "White wins"
        : status === "BlackWon"
            ? "Black wins"
            : status === "InProgress"
                ? ""
                : "Draw";

    function showInvalidMove(message: string) {
        setInvalidMove(message);
        window.clearTimeout(invalidMoveTimerRef.current);
        invalidMoveTimerRef.current = window.setTimeout(() => setInvalidMove(undefined), 2000);
    }

    useEffect(() => {
        const connection = createConnection();
        connectionRef.current = connection;
        connection.on("ReceiveMessage", (message: unknown) => {
            if (typeof message === "string") {
                setInvalidMove(message);
                return;
            }

            if (isSnapshot(message)) {
                setBoard({
                    squares: message.boardSquares.flat().map(square => ({
                        ...square,
                        color: enumName(messageValue(square, "color"), ["White", "Black"]),
                        position: {row: square.position.row, col: square.position.column},
                        piece: square.piece ? {
                            ...square.piece,
                            color: enumName(messageValue(square.piece, "color"), ["White", "Black"]),
                            type: enumName(messageValue(square.piece, "type"), ["Pawn", "Rook", "Knight", "Bishop", "Queen", "King"]),
                            letterId: String(square.piece.letterId ?? "")
                        } : null
                    }))
                })
                setStatus(enumName(message.status, ["InProgress", "WhiteWon", "BlackWon", "DrawByStalemate", "DrawByInsufficientMaterial", "DrawByThreefoldRepetition", "DrawBy75MoveRule"]));
            } else if (isMoveStatus(message)) {
                showInvalidMove(message.resultReason);
            }

        });

        connection.onreconnected(() => startGame(connection, config, gameId));
        connection.start().then(() => startGame(connection, config, gameId)).catch(() => showInvalidMove("Unable to connect to the game server."));

        return () => {
            connection.off("ReceiveMessage");
            void connection.stop();
            window.clearTimeout(invalidMoveTimerRef.current);
        };
    }, [config, gameId]);

    function exitGame() {
        void connectionRef.current?.stop();
        onExit();
    }

    function handleSquareClick(position: Position) {
        if (!board || status !== "InProgress") return;
        const square = board.squares.find(item => item.position.row === position.row && item.position.col === position.col);
        if (!selected) {
            if (square?.piece?.color === config.playerColor) setSelected(position);
            return;
        }

        if (selected.row === position.row && selected.col === position.col) {
            setSelected(undefined);
            return;
        }

        if (square?.piece?.color === config.playerColor) {
            setSelected(position);
            return;
        }

        const selectedSquare = board.squares.find(item => item.position.row === selected.row && item.position.col === selected.col);
        const promotionRow = config.playerColor === "White" ? 0 : 7;
        if (selectedSquare?.piece?.type === "Pawn" && position.row === promotionRow) {
            setPendingPromotion({from: selected, to: position});
            setSelected(undefined);
            return;
        }

        if (connectionRef.current?.state === "Connected") {
            void submitMove(connectionRef.current, {from: selected, to: position});
        }
        setSelected(undefined);
    }

    function promotePiece(piece: PieceType) {
        if (!pendingPromotion || connectionRef.current?.state !== "Connected") return;

        void submitMove(connectionRef.current, {...pendingPromotion, promotion: piece});
        setPendingPromotion(undefined);
    }

    return <main className="relative flex min-h-screen items-center justify-center overflow-hidden bg-[#17221f] p-4 text-[#f5ecd9] sm:p-6">
        <div className="fixed right-4 top-4 z-20 flex flex-col items-end gap-2">
            <button className="border border-[#f1dfc1] bg-[#f1dfc1] px-4 py-2 text-sm font-semibold text-[#3d2b24] hover:bg-[#9b6048] hover:text-[#f1dfc1]" type="button" onClick={exitGame}>
                {gameEnded ? "Back home" : "Exit game"}
            </button>
            {gameEnded && <div className="border border-[#9b6048] bg-[#f1dfc1] px-4 py-3 text-sm text-[#3d2b24] shadow-lg" role="status">
                {resultText}
            </div>}
        </div>
        <section className="relative w-full max-w-[min(92vw,calc(100vh-2rem))]">
            {board ? <Board board={board.squares} playerColor={config.playerColor} selected={selected} onSquareClick={handleSquareClick}/> : <div className="aspect-square w-full animate-pulse bg-[#20302b]"/>}
            {invalidMove && <div className="absolute bottom-4 left-1/2 z-10 flex w-[min(19rem,calc(100%-2rem))] -translate-x-1/2 items-center justify-between gap-4 border border-[#888] bg-[#292929] px-4 py-3 text-sm text-[#f1f1f1] shadow-lg" role="alert" aria-live="assertive">
                <span>{invalidMove}</span>
                <button className="text-lg leading-none text-[#cfcfcf] hover:text-white" type="button" aria-label="Dismiss warning" onClick={() => setInvalidMove(undefined)}>x</button>
            </div>}
        </section>
        {pendingPromotion && <div className="fixed inset-0 z-30 flex items-center justify-center bg-black/50 p-4" role="dialog" aria-modal="true" aria-labelledby="promotion-title">
            <div className="w-full max-w-sm border border-[#9b6048] bg-[#f1dfc1] p-5 text-[#3d2b24] shadow-xl">
                <h2 id="promotion-title" className="text-lg font-semibold">Choose a promotion piece</h2>
                <div className="mt-4 grid grid-cols-2 gap-2">
                    {(["Knight", "Bishop", "Queen", "Rook"] as PieceType[]).map(piece => <button
                        key={piece}
                        className="border border-[#9b6048] bg-[#f1dfc1] px-3 py-2 text-sm font-semibold hover:bg-[#9b6048] hover:text-[#f1dfc1]"
                        type="button"
                        onClick={() => promotePiece(piece)}
                    >
                        {piece}
                    </button>)}
                </div>
                <button className="mt-4 text-sm underline" type="button" onClick={() => setPendingPromotion(undefined)}>Cancel</button>
            </div>
        </div>}
    </main>
}