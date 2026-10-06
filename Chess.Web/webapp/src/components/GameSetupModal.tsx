import {useState, type FormEvent} from "react";
import {isValidClashSetup, standardClashSetup} from "../game/clashSetup.ts";
import ClashPieceSetup from "./ClashPieceSetup.tsx";
import EngineSettingsDialog from "./EngineSettingsDialog.tsx";
import type {
    ChessEngineType,
    ClashPvEConfig,
    ClashPvPConfig,
    Color,
    GameConfig,
    NormalPvEConfig,
    NormalPvPConfig,
    PieceType
} from "../game/types.ts";

type GameSetupModalProps = {
    hasSavedSession: boolean,
    onStartGame: (config: GameConfig) => void,
    onReconnect: () => void
}

const savedGameWarning = "Reconnect to your active game first. To start a new game, resign your current session.";

export default function GameSetupModal({hasSavedSession, onStartGame, onReconnect}: GameSetupModalProps) {
    const [opponent, setOpponent] = useState<"engine" | "multiplayer">("engine");
    const [mode, setMode] = useState<"classic" | "clash">("classic");
    const [color, setColor] = useState<Color>("White");
    const [engine, setEngine] = useState<ChessEngineType>("Stockfish");
    const [elo, setElo] = useState(1400);
    const [depth, setDepth] = useState(20);
    const [moveTime, setMoveTime] = useState(1000);
    const [nodes, setNodes] = useState(1000000);
    const [engineSettingsOpen, setEngineSettingsOpen] = useState(false);
    const [nickname, setNickname] = useState("");
    const [playerPieces, setPlayerPieces] = useState<PieceType[]>(standardClashSetup);
    const [randomizeEnginePieces, setRandomizeEnginePieces] = useState(true);
    const [enginePieces, setEnginePieces] = useState<PieceType[]>(standardClashSetup);

    const isPvE = opponent === "engine";
    const playerSetupValid = mode === "classic" || isValidClashSetup(playerPieces);
    const engineSetupValid = !isPvE || mode !== "clash" || randomizeEnginePieces || isValidClashSetup(enginePieces);
    const depthValid = Number.isInteger(depth) && depth >= 1 && depth <= 50;
    const moveTimeValid = Number.isInteger(moveTime) && moveTime >= 1 && moveTime <= 600000;
    const nodesValid = Number.isInteger(nodes) && nodes >= 1 && nodes <= 1000000000;
    const engineSettingsValid = depthValid && moveTimeValid && nodesValid;
    const eloValid = Number.isInteger(elo) && elo >= 1320 && elo <= 3190;
    const canStart = !hasSavedSession && playerSetupValid && engineSetupValid &&
        (!isPvE || engineSettingsValid && eloValid) && (isPvE || nickname.trim().length > 0);

    function updatePieces(setPieces: (update: (current: PieceType[]) => PieceType[]) => void, index: number, piece: PieceType) {
        setPieces(current => current.map((currentPiece, currentIndex) => currentIndex === index ? piece : currentPiece));
    }

    function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        if (!canStart) return;

        if (isPvE && mode === "classic") {
            const config: NormalPvEConfig = {
                type: "normalPvE", gameMode: "NormalPvE", playerColor: color,
                chessEngineType: engine, elo, depth, moveTime, nodes
            };
            onStartGame(config);
        } else if (isPvE) {
            const config: ClashPvEConfig = {
                type: "clashPvE", gameMode: "ClashPvE", playerColor: color,
                chessEngineType: engine, elo, depth, moveTime, nodes,
                randomizeChessEnginePieces: randomizeEnginePieces,
                playerPieces,
                chessEnginePieces: randomizeEnginePieces ? null : enginePieces
            };
            onStartGame(config);
        } else if (mode === "classic") {
            const config: NormalPvPConfig = {
                type: "normalPvP", gameMode: "NormalPvP", nickname: nickname.trim()
            };
            onStartGame(config);
        } else {
            const config: ClashPvPConfig = {
                type: "clashPvP", gameMode: "ClashPvP", nickname: nickname.trim(), playerPieces
            };
            onStartGame(config);
        }
    }

    return <section className="max-h-[calc(100dvh-2rem)] w-full max-w-2xl overflow-y-auto border border-[#9b6048] bg-[#252525] p-5 shadow-2xl sm:p-8"
        role="dialog" aria-modal="true" aria-labelledby="setup-title">
        <header className="mb-6 flex flex-wrap items-end justify-between gap-3 border-b border-[#574136] pb-4">
            <div>
                <p className="text-xs uppercase text-[#c78b6e]">Chess</p>
                <h1 id="setup-title" className="mt-1 text-2xl font-semibold">Set up a game</h1>
            </div>
            {hasSavedSession && <button
                className="border border-[#f1dfc1] bg-[#f1dfc1] px-4 py-2 text-sm font-semibold text-[#3d2b24] hover:bg-[#9b6048] hover:text-[#f1dfc1]"
                type="button"
                onClick={onReconnect}
            >Reconnect game</button>}
        </header>

        <form onSubmit={handleSubmit} className="space-y-5">
            <fieldset className="border border-[#9b6048] p-4">
                <legend className="px-2 text-sm">Opponent</legend>
                <div className="grid grid-cols-2 gap-2">
                    <label className={`flex cursor-pointer items-center gap-2 border px-3 py-2 text-sm ${isPvE ? "border-[#f1dfc1] bg-[#3a322c]" : "border-[#574136]"}`}>
                        <input type="radio" name="opponent" checked={isPvE} onChange={() => setOpponent("engine")} />
                        Chess engine
                    </label>
                    <label className={`flex cursor-pointer items-center gap-2 border px-3 py-2 text-sm ${!isPvE ? "border-[#f1dfc1] bg-[#3a322c]" : "border-[#574136]"}`}>
                        <input type="radio" name="opponent" checked={!isPvE} onChange={() => setOpponent("multiplayer")} />
                        Multiplayer
                    </label>
                </div>
            </fieldset>

            <fieldset className="border border-[#9b6048] p-4">
                <legend className="px-2 text-sm">Game mode</legend>
                <div className="grid grid-cols-2 gap-2">
                    <label className={`flex cursor-pointer items-center gap-2 border px-3 py-2 text-sm ${mode === "classic" ? "border-[#f1dfc1] bg-[#3a322c]" : "border-[#574136]"}`}>
                        <input type="radio" name="mode" checked={mode === "classic"} onChange={() => setMode("classic")} />
                        Classic
                    </label>
                    <label className={`flex cursor-pointer items-center gap-2 border px-3 py-2 text-sm ${mode === "clash" ? "border-[#f1dfc1] bg-[#3a322c]" : "border-[#574136]"}`}>
                        <input type="radio" name="mode" checked={mode === "clash"} onChange={() => setMode("clash")} />
                        Clash
                    </label>
                </div>
            </fieldset>

            {isPvE ? <>
                <div className="grid gap-4 sm:grid-cols-2">
                    <fieldset className="border border-[#9b6048] p-4">
                        <legend className="px-2 text-sm">Play as</legend>
                        <div className="mt-2 flex gap-6">
                            {(["White", "Black"] as Color[]).map(value => <label key={value} className="flex items-center gap-2 text-sm">
                                <input type="radio" name="color" checked={color === value} onChange={() => setColor(value)} />
                                {value}
                            </label>)}
                        </div>
                    </fieldset>
                    <fieldset className="border border-[#9b6048] p-4">
                        <legend className="px-2 text-sm">Engine</legend>
                        <select className="mt-2 w-full border border-[#9b6048] bg-[#f1dfc1] px-2 py-2 text-[#202020] outline-none focus:border-white"
                            value={engine} onChange={event => setEngine(event.target.value as ChessEngineType)}>
                            <option value="Stockfish">Stockfish</option>
                            <option value="Deakfish" disabled>Deakfish</option>
                        </select>
                    </fieldset>
                </div>
                <div className="grid gap-3 sm:grid-cols-2">
                    <fieldset className="border border-[#9b6048] p-4">
                        <legend className="px-2 text-sm">Engine Elo</legend>
                        <input className="mt-2 w-full border border-[#9b6048] bg-[#f1dfc1] px-3 py-2 text-[#202020] outline-none focus:border-white"
                            type="number" value={elo} onChange={event => setElo(Number(event.target.value))}
                            min={1320} max={3190} step={1} required aria-invalid={!eloValid}
                            aria-describedby={!eloValid ? "engine-elo-error" : undefined} />
                        {!eloValid && <span id="engine-elo-error" className="mt-1 block text-xs text-[#f0a58e]" role="alert">
                            Enter a whole number from 1320 to 3190.
                        </span>}
                    </fieldset>
                    <fieldset className="border border-[#9b6048] p-3">
                        <legend className="px-2 text-sm">Engine settings</legend>
                        <button className="w-full px-1 py-1 text-left text-sm font-semibold hover:bg-[#3a322c] focus:outline-2 focus:outline-[#f1dfc1]"
                            type="button" aria-haspopup="dialog" aria-expanded={engineSettingsOpen}
                            onClick={() => setEngineSettingsOpen(true)}>
                            Configure limits
                            <span className="mt-1 block text-xs font-normal text-[#d5c2a5]">Depth, move time, and nodes</span>
                        </button>
                    </fieldset>
                </div>
                {isPvE && (!engineSettingsValid || !eloValid) && <p className="text-xs text-[#f0a58e]" role="alert">
                    Correct the highlighted engine values before starting.
                </p>}
            </> : <fieldset className="border border-[#9b6048] p-4">
                <legend className="px-2 text-sm">Nickname</legend>
                <input className="mt-2 w-full border border-[#9b6048] bg-[#f1dfc1] px-3 py-2 text-[#202020] outline-none focus:border-white"
                    type="text" value={nickname} onChange={event => setNickname(event.target.value)} maxLength={32} required />
            </fieldset>}

            {mode === "clash" && <div className="space-y-4">
                <ClashPieceSetup label="Your two ranks" pieces={playerPieces}
                    onChange={(index, piece) => updatePieces(setPlayerPieces, index, piece)} />
                {isPvE && <fieldset className="border border-[#9b6048] p-4">
                    <legend className="px-2 text-sm">Engine setup</legend>
                    <div className="mb-4 flex flex-wrap gap-x-5 gap-y-2 text-sm">
                        <label className="flex items-center gap-2">
                            <input type="radio" name="engineSetup" checked={randomizeEnginePieces} onChange={() => setRandomizeEnginePieces(true)} />
                            Randomize
                        </label>
                        <label className="flex items-center gap-2">
                            <input type="radio" name="engineSetup" checked={!randomizeEnginePieces} onChange={() => setRandomizeEnginePieces(false)} />
                            Custom
                        </label>
                    </div>
                    {!randomizeEnginePieces && <ClashPieceSetup label="Engine's two ranks" pieces={enginePieces}
                        onChange={(index, piece) => updatePieces(setEnginePieces, index, piece)} />}
                </fieldset>}
            </div>}

            <div className="flex flex-col items-center gap-3 pt-1 text-center">
                <span title={hasSavedSession ? savedGameWarning : undefined} className="inline-flex">
                    <button className="border border-[#f1dfc1] bg-[#f1dfc1] px-5 py-2.5 font-semibold text-[#3d2b24] hover:bg-[#9b6048] hover:text-[#f1dfc1] disabled:cursor-not-allowed disabled:opacity-45"
                        type="submit" disabled={!canStart}>Start game</button>
                </span>
                {hasSavedSession && <span className="text-xs text-[#d5c2a5]">Reconnect or resign your active session first.</span>}
                {(!playerSetupValid || !engineSetupValid) && <span className="text-xs text-[#f0a58e]">Complete a valid clash setup to continue.</span>}
            </div>
        </form>
        <EngineSettingsDialog
            open={engineSettingsOpen}
            depth={depth}
            moveTime={moveTime}
            nodes={nodes}
            depthValid={depthValid}
            moveTimeValid={moveTimeValid}
            nodesValid={nodesValid}
            engineSettingsValid={engineSettingsValid}
            onDepthChange={setDepth}
            onMoveTimeChange={setMoveTime}
            onNodesChange={setNodes}
            onClose={() => setEngineSettingsOpen(false)}
        />
    </section>;
}