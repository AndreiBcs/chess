import SquareComponent from "./Square";
import type { Square } from "../game/types.ts";

type BoardProps = {
    board: Square[];
};

export default function Board({ board }: BoardProps) {
    
    const squares = board.map((square) => (
        <SquareComponent
            key={`${square.position.row}-${square.position.col}`}
            color={square.color}
            position={square.position}
            piece={square.piece}
            onClick={(position) => console.log(position)}
        />
    ));

    return (
        <div className="grid grid-cols-8">
            {squares}
        </div>
    );
}