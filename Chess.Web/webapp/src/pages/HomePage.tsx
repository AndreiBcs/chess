import {useState} from "react";
import type {Color, EngineType, GameConfig} from "../game/types.ts";

interface HomePageProps {
    onStartGame: (config: GameConfig) => void
}

export default function HomePage({onStartGame}: HomePageProps) {

    const [color, setColor] = useState<Color>("White");
    const [engine, setEngine] = useState<EngineType>("Stockfish")
    const [elo, setElo] = useState(1400)

    function handleSubmit(e: { preventDefault: () => void; }) {
        e.preventDefault()

        console.log({color, engine, elo})
        
        return onStartGame({
            playerColor: color,
            engineType: engine,
            elo: elo
        })
    }

    return <main className="flex min-h-screen items-center justify-center bg-[#151515] px-4 py-8 text-[#f1dfc1] sm:px-6">
        <form onSubmit={handleSubmit} className="w-full max-w-md border border-[#9b6048] bg-[#252525] p-6 shadow-xl sm:p-8">
            <div className="space-y-5">
            <fieldset className="border border-[#9b6048] p-4">
                <legend className="px-2 text-sm text-[#f1dfc1]">Play as</legend>
                <div className="mt-2 flex gap-6">
                <label className="flex items-center gap-2">
                    <input
                        type="radio"
                        value="white"
                        checked={color === "White"}
                        onChange={() => setColor("White")}
                    />
                    White
                </label>
                <label className="flex items-center gap-2">
                    <input
                        type="radio"
                        value="black"
                        checked={color === "Black"}
                        onChange={() => setColor("Black")}
                    />
                    Black
                </label>
                </div>
            </fieldset>

            <fieldset className="border border-[#9b6048] p-4">
                <legend className="px-2 text-sm text-[#f1dfc1]">Engine</legend>
                <select
                    className="mt-2 w-full border border-[#9b6048] bg-[#f1dfc1] px-2 py-2 text-[#202020] outline-none focus:border-white"
                    value={engine}
                    onChange={e => setEngine(e.target.value as EngineType)}
                >
                    <option value="Stockfish">Stockfish</option>
                    <option value="Deakfish" disabled>Deakfish</option>
                </select>
            </fieldset>

            <fieldset className="border border-[#9b6048] p-4">
                <legend className="px-2 text-sm text-[#f1dfc1]">Elo</legend>
                <input className="mt-2 w-full border border-[#9b6048] bg-[#f1dfc1] px-2 py-2 text-[#202020] outline-none focus:border-white"
                    type="number"
                    value={elo}
                    onChange={e => setElo(Number(e.target.value))}
                    min={1320}
                    max={3190}
                />
            </fieldset>
            </div>

            <button className="mt-7 w-full border border-[#f1dfc1] bg-[#f1dfc1] px-4 py-2 font-semibold text-[#9b6048] hover:bg-[#9b6048] hover:text-[#f1dfc1]" type="submit">Start game</button>
        </form>
    </main>
}