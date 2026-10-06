import GameSetupModal from "../components/GameSetupModal.tsx";
import type {GameConfig} from "../game/types.ts";

type HomePageProps = {
    hasSavedSession: boolean,
    onStartGame: (config: GameConfig) => void,
    onReconnect: () => void
}

export default function HomePage({hasSavedSession, onStartGame, onReconnect}: HomePageProps) {
    return <main className="flex min-h-screen items-center justify-center bg-[#151515] px-4 py-8 text-[#f1dfc1] sm:px-6">
        <GameSetupModal
            hasSavedSession={hasSavedSession}
            onStartGame={onStartGame}
            onReconnect={onReconnect}
        />
    </main>
}