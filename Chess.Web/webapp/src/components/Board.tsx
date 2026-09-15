import SquareComponent from "./Square";
import type { Color, Square } from "../game/types.ts";

type BoardProps = {
    board: Square[];
    playerColor: Color;
    selected?: {row: number, col: number};
    onSquareClick: (position: {row: number, col: number}) => void;
};

export default function Board({ board, playerColor, selected, onSquareClick }: BoardProps) {
    
    const squares = board.map((square) => (
        <SquareComponent
            key={`${square.position.row}-${square.position.col}`}
            color={square.color}
            position={square.position}
            piece={square.piece}
            rotated={playerColor === "Black"}
            selected={selected?.row === square.position.row && selected.col === square.position.col}
            onClick={onSquareClick}
        />
    ));

    return (
        <div aria-label="Chess board" className={`grid aspect-square w-full grid-cols-8 overflow-hidden border-4 border-[#3d2b24] shadow-2xl ${playerColor === "Black" ? "rotate-180" : ""}`}>
            {squares}
        </div>
    );
}