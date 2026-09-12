import {createSocket, sendStartOptions, sendMove} from "../api/index.ts"
import {useEffect, useRef, useState} from "react";
import Board from "../components/Board.tsx";
import type {GameConfig, Board as BoardState, GameStatus, PieceType, Position} from "../game/types.ts";

type PendingPromotion = {
    from: Position;
    to: Position;
};

export default function GamePage ({config, gameId, onExit}: {config: GameConfig, gameId: string, onExit: () => void}) {
    const [board, setBoard] = useState<BoardState>();
    const [status, setStatus] = useState<GameStatus>("InProgress");
    const [selected, setSelected] = useState<Position>();
    const [pendingPromotion, setPendingPromotion] = useState<PendingPromotion>();
    const [invalidMove, setInvalidMove] = useState<string>();
    const socketRef = useRef<WebSocket | null>(null);
    const invalidMoveTimerRef = useRef<number | undefined>(undefined);

    const gameEnded = status !== "InProgress";
    const resultText = status === "WhiteWon"
        ? "White wins"
        : status === "BlackWon"
            ? "Black wins"
            : status === "InProgress"
                ? ""
                : "Draw";

    useEffect(() => {
        const socket = createSocket();
        socketRef.current = socket;
        socket.onopen = () => {
            sendStartOptions(config, gameId, socket);
        }

        socket.onmessage = (e) => {
            const message = JSON.parse(e.data);

            if (message.type === "Snapshot"){
                setBoard({
                    squares: message.boardSquares.flat().map((square: {position: {row: number, column: number}}) => ({
                        ...square,
                        position: {row: square.position.row, col: square.position.column},
                    }))
                })
                setStatus(message.status);
            } else if (message.type === "MoveResult") {
                if (message.result === "Invalid") {
                    setInvalidMove("The move is not valid");
                    window.clearTimeout(invalidMoveTimerRef.current);
                    invalidMoveTimerRef.current = window.setTimeout(() => setInvalidMove(undefined), 2000);
                }
            }
        }
        return () => {
            socket.close();
            window.clearTimeout(invalidMoveTimerRef.current);
        };
    }, [config, gameId]);

    function exitGame() {
        socketRef.current?.close();
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

        if (socketRef.current?.readyState === WebSocket.OPEN) {
            sendMove({from: selected, to: position}, socketRef.current);
        }
        setSelected(undefined);
    }

    function promotePiece(piece: PieceType) {
        if (!pendingPromotion || socketRef.current?.readyState !== WebSocket.OPEN) return;

        sendMove({...pendingPromotion, promotion: piece}, socketRef.current);
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