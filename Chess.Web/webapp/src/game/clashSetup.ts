import type {PieceType} from "./types.ts";

const pieceTypes: PieceType[] = ["Pawn", "Rook", "Knight", "Bishop", "Queen", "King"];

export const standardClashSetup: PieceType[] = [
    "Pawn", "Pawn", "Pawn", "Pawn", "Pawn", "Pawn", "Pawn", "Pawn",
    "King", "Rook", "Knight", "Bishop", "Queen", "Bishop", "Knight", "Rook"
];

export function isValidClashSetup(pieces: PieceType[]) {
    const expectedCounts: Record<PieceType, number> = {
        Pawn: 8,
        Rook: 2,
        Knight: 2,
        Bishop: 2,
        Queen: 1,
        King: 1
    };
    return pieces.length === 16 && pieces.slice(8).includes("King") &&
        pieceTypes.every(piece => pieces.filter(current => current === piece).length === expectedCounts[piece]);
}