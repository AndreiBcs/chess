import type {Color, GameConfig} from "./types.ts";

export type StoredGameSession = {
    config: GameConfig,
    token: string,
    playerColor: Color
}

const activeSessionKey = "chess.activeSession";

export function readStoredGameSession(): StoredGameSession | undefined {
    const value = localStorage.getItem(activeSessionKey);
    if (!value) return undefined;

    try {
        const session = JSON.parse(value) as StoredGameSession;
        if (!session.token || !session.config || !session.playerColor) return undefined;
        return session;
    } catch {
        localStorage.removeItem(activeSessionKey);
        return undefined;
    }
}

export function storeGameSession(session: StoredGameSession) {
    localStorage.setItem(activeSessionKey, JSON.stringify(session));
}

export function clearStoredGameSession() {
    localStorage.removeItem(activeSessionKey);
}