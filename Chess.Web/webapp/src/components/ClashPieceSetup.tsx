import type {PieceType} from "../game/types.ts";
import {isValidClashSetup} from "../game/clashSetup.ts";

const pieceTypes: PieceType[] = ["Pawn", "Rook", "Knight", "Bishop", "Queen", "King"];

export default function ClashPieceSetup({label, pieces, onChange}: {
    label: string,
    pieces: PieceType[],
    onChange: (index: number, piece: PieceType) => void
}) {
    const valid = isValidClashSetup(pieces);

    return <fieldset className="border border-[#9b6048] p-3">
        <legend className="px-2 text-sm">{label}</legend>
        <div className="mt-2 grid grid-cols-8 gap-1.5">
            {pieces.map((piece, index) => <label key={index} className="min-w-0">
                <span className="sr-only">{index < 8 ? "Upper" : "Lower"} rank, file {index % 8 + 1}</span>
                <select
                    className="aspect-square w-full min-w-0 border border-[#9b6048] bg-[#f1dfc1] px-0.5 text-center text-[10px] text-[#202020] outline-none focus:border-white sm:text-xs"
                    value={piece}
                    onChange={event => onChange(index, event.target.value as PieceType)}
                >
                    {pieceTypes.map(type => <option key={type} value={type}>{type}</option>)}
                </select>
            </label>)}
        </div>
        {!valid && <p className="mt-2 text-xs text-[#f0a58e]" role="alert">
            Use 8 pawns, 2 rooks, 2 knights, 2 bishops, and 1 each queen and king. The king must be on the lower rank.
        </p>}
    </fieldset>;
}