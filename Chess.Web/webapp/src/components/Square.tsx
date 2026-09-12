import bishopBlack from "../assets/bishop_black.png";
import bishopWhite from "../assets/bishop_white.png";
import kingBlack from "../assets/king_black.png";
import kingWhite from "../assets/king_white.png";
import knightBlack from "../assets/knight_black.png";
import knightWhite from "../assets/knight_white.png";
import pawnBlack from "../assets/pawn_black.png";
import pawnWhite from "../assets/pawn_white.png";
import queenBlack from "../assets/queen_black.png";
import queenWhite from "../assets/queen_white.png";
import rookBlack from "../assets/rook_black.png";
import rookWhite from "../assets/rook_white.png";
import type { Color, Piece, PieceType, Position } from "../game/types.ts";

type SquareProps = {
    color: Color;
    position: Position;
    piece: Piece | null;
    rotated?: boolean;
    selected?: boolean;
    onClick: (position: Position) => void;
};

export default function Square({color, position, piece, rotated, selected, onClick}: SquareProps) {
    const backgroundColor = color === "White" ? "bg-[#f1dfc1]" : "bg-[#9b6048]";
    const pieceImages: Record<Color, Record<PieceType, string>> = {
        White: {Pawn: pawnWhite, Rook: rookWhite, Knight: knightWhite, Bishop: bishopWhite, Queen: queenWhite, King: kingWhite},
        Black: {Pawn: pawnBlack, Rook: rookBlack, Knight: knightBlack, Bishop: bishopBlack, Queen: queenBlack, King: kingBlack},
    };
    
    return (
        <button
            className={`aspect-square w-full ${backgroundColor} flex items-center justify-center focus:z-10 focus:outline-4 focus:outline-[#e9b44c] ${rotated ? "rotate-180" : ""} ${selected ? "ring-inset ring-4 ring-[#e9b44c]" : ""}`}
            onClick={() => onClick(position)}
        >
            {piece && (
                <img
                    src={pieceImages[piece.color][piece.type]}
                    alt={piece.letterId}
                    className="h-full w-full object-contain"
                />
            )}
        </button>
    );
}