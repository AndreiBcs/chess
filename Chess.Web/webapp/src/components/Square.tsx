import type { Color, Piece, Position } from "../game/types.ts";

type SquareProps = {
    color: Color;
    position: Position;
    piece: Piece | null;
    onClick: (position: Position) => void;
};

export default function Square({color, position, piece, onClick}: SquareProps) {

    const backgroundColor = color === "White" ? "bg-amber-100" : "bg-amber-700";
    
    return (
        <button
            className={`aspect-square w-full ${backgroundColor} flex items-center justify-center`}
            onClick={() => onClick(position)}
        >
            {piece && (
                <img
                    src={piece.filePath}
                    alt={piece.letterId}
                    className="h-full w-full object-contain"
                />
            )}
        </button>
    );
}