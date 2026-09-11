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

    return <>
        <form onSubmit={handleSubmit}>
            <fieldset>
                <legend>Color</legend>
                <label>
                    <input
                        type="radio"
                        value="white"
                        checked={color === "White"}
                        onChange={() => setColor("White")}
                    />
                    White
                </label>
                <label>
                    <input
                        type="radio"
                        value="black"
                        checked={color === "Black"}
                        onChange={() => setColor("Black")}
                    />
                    Black
                </label>
            </fieldset>

            <fieldset>
                <legend>Chess Engine</legend>
                <label>
                    <input
                        type="radio"
                        value="stockfish"
                        checked={engine === "Stockfish"}
                        onChange={() => setEngine("Stockfish")}
                    />
                    Stockfish
                </label>
            </fieldset>

            <label>
                Elo
                <input
                    type="number"
                    value={elo}
                    onChange={e => setElo(Number(e.target.value))}
                    min={1320}
                    max={3190}
                />
            </label>

            <button type="submit">Start Game</button>
        </form>
    </>
}