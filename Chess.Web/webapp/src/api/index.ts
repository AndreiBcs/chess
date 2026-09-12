import type {GameConfig, Move} from "../game/types.ts"

export function createSocket() {
    const protocol = window.location.protocol === "https:" ? "wss:" : "ws:";
    return new WebSocket(`${protocol}//${window.location.host}/ws`);
}

export function sendMove(move: Move, socket: WebSocket) {
    socket.send(JSON.stringify({
        type: "Move",
        data: {
            from: { row: move.from.row, column: move.from.col },
            to: { row: move.to.row, column: move.to.col },
            promotion: move.promotion
        }
    }));
}

export function sendStartOptions(config: GameConfig, gameId: string, socket: WebSocket) {
    socket.send(JSON.stringify({
        type: "StartOptions",
        data: {
            playerColor: config.playerColor,
            engineType: config.engineType,
            elo: config.elo,
            gameId
        }
    }));
}