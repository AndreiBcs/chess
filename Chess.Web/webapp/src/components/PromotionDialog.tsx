import type {PieceType} from "../game/types.ts";

export default function PromotionDialog({onPromote, onCancel}: {
    onPromote: (piece: PieceType) => void,
    onCancel: () => void
}) {
    const promotionPieces: PieceType[] = ["Knight", "Bishop", "Queen", "Rook"];

    return <div className="fixed inset-0 z-30 flex items-center justify-center bg-black/50 p-4"
        role="dialog" aria-modal="true" aria-labelledby="promotion-title">
        <section className="w-full max-w-sm border border-[#9b6048] bg-[#f1dfc1] p-5 text-[#3d2b24] shadow-xl">
            <h2 id="promotion-title" className="text-lg font-semibold">Choose a promotion piece</h2>
            <div className="mt-4 grid grid-cols-2 gap-2">
                {promotionPieces.map(piece => <button key={piece}
                    className="border border-[#9b6048] bg-[#f1dfc1] px-3 py-2 text-sm font-semibold hover:bg-[#9b6048] hover:text-[#f1dfc1]"
                    type="button" onClick={() => onPromote(piece)}>{piece}</button>)}
            </div>
            <button className="mt-4 text-sm underline" type="button" onClick={onCancel}>Cancel</button>
        </section>
    </div>;
}