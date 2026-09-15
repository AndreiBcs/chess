import { HubConnectionBuilder, LogLevel, type HubConnection } from "@microsoft/signalr";
import type { GameConfig, Move } from "../game/types.ts";

const hubUrl = import.meta.env.VITE_SIGNALR_URL || "/gamehub";

export function createConnection() {
    return new HubConnectionBuilder()
        .withUrl(hubUrl)
        .withAutomaticReconnect()
        .configureLogging(import.meta.env.DEV ? LogLevel.Warning : LogLevel.Error)
        .build();
}

export function startGame(connection: HubConnection, config: GameConfig, gameId: string) {
    return connection.invoke("StartGame", {
        playerColor: config.playerColor,
        engineType: config.engineType,
        elo: config.elo,
        gameId
    });
}

export function submitMove(connection: HubConnection, move: Move) {
    return connection.invoke("SubmitMove", {
        from: { row: move.from.row, column: move.from.col },
        to: { row: move.to.row, column: move.to.col },
        promotion: move.promotion?.toLowerCase() ?? null
    });
}