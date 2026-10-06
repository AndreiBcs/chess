import SquareComponent from "./Square";
import type { Color, Position, Board } from "../game/types.ts";

type BoardProps = {
    board: Board;
    playerColor: Color;
    selected?: Position;
    onSquareClick: (position: Position) => void;
};

export default function Board({ board, playerColor, selected, onSquareClick }: BoardProps) {
    
    const squares = board.map((square) => (
        <SquareComponent
            key={`${square.position.row}-${square.position.col}`}
            square={square}
            rotated={playerColor === "Black"}
            selected={selected !== undefined && selected.row === square.position.row && selected.col === square.position.col}
            onClick={onSquareClick}
        />
    ));

    return (
        <div aria-label="Chess board" 
             className={`grid aspect-square w-full grid-cols-8
              overflow-hidden border-4 border-[#3d2b24] shadow-2xl 
              ${playerColor === "Black" ? "rotate-180" : ""}`}
        >
            {squares}
        </div>
    );
}