export default function GameResultDialog({result, onNewGame, onHome}: {
    result: string,
    onNewGame: () => void,
    onHome: () => void
}) {
    return <div className="fixed inset-0 z-40 flex items-center justify-center bg-black/70 p-4"
        role="dialog" aria-modal="true" aria-labelledby="game-result-title">
        <section className="w-full max-w-md border border-[#9b6048] bg-[#252525] p-6 text-[#f5ecd9] shadow-2xl">
            <p className="text-xs uppercase text-[#c78b6e]">Game complete</p>
            <h2 id="game-result-title" className="mt-2 text-2xl font-semibold">{result}</h2>
            <div className="mt-6 grid gap-3 sm:grid-cols-2">
                <button className="border border-[#f1dfc1] bg-[#f1dfc1] px-4 py-2.5 font-semibold text-[#3d2b24] hover:bg-[#9b6048] hover:text-[#f1dfc1]"
                    type="button" onClick={onNewGame}>New game</button>
                <button className="border border-[#9b6048] px-4 py-2.5 font-semibold hover:bg-[#3a322c]"
                    type="button" onClick={onHome}>Home</button>
            </div>
        </section>
    </div>;
}